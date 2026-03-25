using Comfort.Common;
using Fika.Core.Main.Components;
using Fika.Core.Main.Custom;
using Fika.Core.Main.Utils;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using FPVDroneModClient.Components.Base;
using FPVDroneModFika.Data;
using FPVDroneModFika.Packets;
using System;
using UnityEngine;

namespace FPVDroneModFika.Components
{
    public class DroneSyncComponent : ThrottledMono
    {
        public override float UpdateRate { get; } = 30;
        public BaseDroneController DroneController;
        public int NetId;
        private DronePositionData _positionData;

        protected override void Awake()
        {
            base.Awake();
            
            DroneController = GetComponent<BaseDroneController>();
            DroneController.RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            
            _positionData = new DronePositionData();
        }

        public override void Tick()
        {
            // if there are any fika position syncers nuke them
            if (FikaBackendUtils.IsServer)
            {
                var syncer = GetComponent<ItemPositionSyncer>();

                if (syncer != null)
                {
                    Destroy(syncer);
                }
            }

            // only we can send pos packets cuz we are the owner!
            if (DroneController.Owner.IsYourPlayer && DroneController.RigidBody && !DroneController.IsAboutToBeDestroyed)
            {
                IFikaNetworkManager manager = Singleton<IFikaNetworkManager>.Instance;

                _positionData.DroneNetId = NetId;
                _positionData.Thrust = DroneController.Thrust;
                _positionData.Position = DroneController.RigidBody.position;
                _positionData.Rotation = DroneController.RigidBody.rotation;
                _positionData.Velocity = Vector3.zero;
                _positionData.AngularVelocity = Vector3.zero;

                DronePositionPacket packet = new DronePositionPacket()
                {
                    Data = _positionData
                };

                manager.SendData(ref packet, DeliveryMethod.Unreliable, true);
            }
        }

        public void SyncDronePosition(DronePositionPacket packet)
        {
            if (DroneController.IsAboutToBeDestroyed) return;
            
            DronePositionData data = packet.Data;
            Rigidbody rb = DroneController.RigidBody;

            // DebugLogger.LogInfo($"{data.Position.x} {data.Position.y} {data.Position.z} | {data.Thrust}");
            rb.position = data.Position;
            rb.rotation = data.Rotation;
            rb.velocity = data.Velocity;
            rb.angularVelocity = data.AngularVelocity;
            DroneController.Thrust = data.Thrust;
        }

        public void SyncDroneControl(DroneControlPacket packet)
        {
            DroneControlData data = packet.Data;

            if (data.IsDroneControlled)
            {
                DroneController.OnPilotEnter(false);
            }
            else
            {
                DroneController.OnPilotExit(false);
            }
        }
    }
}
