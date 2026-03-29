using Comfort.Common;
using Fika.Core.Main.Components;
using Fika.Core.Main.Custom;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Helpers;
using FPVDroneModFika.Data;
using FPVDroneModFika.Packets;
using System.Collections.Generic;
using UnityEngine;

namespace FPVDroneModFika.Components
{
    public class DroneSyncComponent : ThrottledMono
    {
        public override float UpdateRate { get; } = 30;
        public BaseDroneController DroneController;
        public int NetId;
        private DronePositionData _currentPositionData;
        
        private List<DronePositionData> _buffer = [];
        private float _timeOffset = 0f;
        
        private float _interpDelay = 0.1f;
        private float _renderTime => Time.realtimeSinceStartup - _interpDelay;

        protected override void Awake()
        {
            base.Awake();

            DroneController = GetComponent<BaseDroneController>();
            
            _currentPositionData = new DronePositionData();
        }

        private void Update()
        {
            if (!DroneController.Owner.IsYourPlayer)
            {
                DronePositionData a = default;
                DronePositionData b = default;

                for (int i = 0; i < _buffer.Count; i++)
                {
                    if (_buffer[i].Time <= _renderTime && _buffer[i + 1].Time >= _renderTime)
                    {
                        a = _buffer[i];
                        b = _buffer[i + 1];
                        break;
                    }
                }

                float t = Mathf.InverseLerp(a.Time, b.Time, _renderTime);

                Vector3 position = Vector3.Lerp(a.Position, b.Position, t);
                //Quaternion rotation = Quaternion.Slerp(a.Rotation.normalized, b.Rotation.normalized, t);

                DroneController.RigidBody.position = position;
                ///DroneController.RigidBody.rotation = rotation;
            }
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
                DebugLogger.LogInfo("sending pos packet");
                
                IFikaNetworkManager manager = Singleton<IFikaNetworkManager>.Instance;

                _currentPositionData.DroneNetId = NetId;
                _currentPositionData.Thrust = DroneController.Thrust;
                _currentPositionData.Position = rb.position;
                _currentPositionData.Rotation = rb.rotation;
                _currentPositionData.Velocity = rb.velocity;
                _currentPositionData.AngularVelocity = rb.angularVelocity;
                _currentPositionData.Timestamp = Time.realtimeSinceStartup;

                DronePositionPacket packet = new DronePositionPacket
                {
                    DroneNetId = _currentPositionData.DroneNetId,
                    Thrust = _currentPositionData.Thrust,
                    Position = _currentPositionData.Position,
                    Rotation = _currentPositionData.Rotation,
                    Velocity = _currentPositionData.Velocity,
                    AngularVelocity = _currentPositionData.AngularVelocity,
                    Timestamp = _currentPositionData.Timestamp
                };

                manager.SendData(ref packet, DeliveryMethod.Unreliable, true);
            }
        }

        public void SyncDronePosition(DronePositionPacket packet)
        {
            if (DroneController.IsAboutToBeDestroyed) return;
            
            Rigidbody rb = DroneController.RigidBody;

            DebugLogger.LogInfo($"{packet.Position.x} {packet.Position.y} {packet.Position.z} | {packet.Thrust}");
            /*
            rb.position = packet.Position;
            rb.rotation = packet.Rotation;
            rb.velocity = packet.Velocity;
            rb.angularVelocity = packet.AngularVelocity;
            */
            
            DroneController.Thrust = packet.Thrust;

            float time = Time.realtimeSinceStartup;

            _timeOffset = time - packet.Timestamp;
            float adjusted = packet.Timestamp + _timeOffset;
            
            _buffer.Add(new DronePositionData
            {
                DroneNetId = packet.DroneNetId,
                Thrust = packet.Thrust,
                Position = packet.Position,
                Rotation = packet.Rotation,
                Velocity = packet.Velocity,
                AngularVelocity = packet.AngularVelocity,
                Timestamp = packet.Timestamp,
                Time = adjusted
            });
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
