using Comfort.Common;
using Fika.Core.Main.Components;
using Fika.Core.Main.Custom;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using FPVDroneModClient.Components.Base;
using FPVDroneModFika.Data;
using FPVDroneModFika.Packets;
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
            // nuke any fika item position syncers
            ItemPositionSyncer syncer = GetComponent<ItemPositionSyncer>();
            if (syncer)
            {
                Destroy(syncer);
            }
            
            // only we can send pos packets cuz we are the owner!
            Rigidbody rb = DroneController.RigidBody;
            if (DroneController.Owner.IsYourPlayer && rb && !DroneController.IsAboutToBeDestroyed)
            {
                IFikaNetworkManager manager = Singleton<IFikaNetworkManager>.Instance;

                _positionData.DroneNetId = NetId;
                _positionData.Thrust = DroneController.Thrust;
                _positionData.Position = rb.position;
                _positionData.Rotation = rb.rotation;
                _positionData.Velocity = rb.velocity;
                _positionData.AngularVelocity = rb.angularVelocity;

                DronePositionPacket packet = new DronePositionPacket
                {
                    DroneNetId = _positionData.DroneNetId,
                    Thrust = _positionData.Thrust,
                    Position = _positionData.Position,
                    Rotation = _positionData.Rotation,
                    Velocity = _positionData.Velocity,
                    AngularVelocity = _positionData.AngularVelocity
                };

                manager.SendData(ref packet, DeliveryMethod.Unreliable, true);
            }
        }

        public void SyncDronePosition(DronePositionPacket packet)
        {
            if (DroneController.IsAboutToBeDestroyed) return;
            
            Rigidbody rb = DroneController.RigidBody;
            if (rb)
            {
                rb.position = packet.Position;
                rb.rotation = packet.Rotation;
                rb.velocity = packet.Velocity;
                rb.angularVelocity = packet.AngularVelocity;
                DroneController.Thrust = packet.Thrust;
            }
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
