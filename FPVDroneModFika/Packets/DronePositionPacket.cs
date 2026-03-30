using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib.Utils;
using FPVDroneModFika.Data;
using UnityEngine;

namespace FPVDroneModFika.Packets
{
    public struct DronePositionPacket : INetSerializable
    {
        public int DroneNetId;
        public float Thrust;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(DroneNetId);
            writer.PutPackedFloat(Thrust, 0f, 1f);
            writer.PutUnmanaged(Position);
            writer.PutUnmanaged(Rotation);
            writer.PutUnmanaged(Velocity);
            writer.PutUnmanaged(AngularVelocity);
        }

        public void Deserialize(NetDataReader reader)
        {
            DroneNetId = reader.GetInt();
            Thrust = reader.GetPackedFloat(0f, 1f);
            Position = reader.GetUnmanaged<Vector3>();
            Rotation = reader.GetUnmanaged<Quaternion>();
            Velocity = reader.GetUnmanaged<Vector3>();
            AngularVelocity = reader.GetUnmanaged<Vector3>();
        }
    }
}
