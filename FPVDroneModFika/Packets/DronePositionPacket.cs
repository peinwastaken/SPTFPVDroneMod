using Fika.Core.Networking.LiteNetLib.Utils;
using FPVDroneModFika.Data;
using UnityEngine;

namespace FPVDroneModFika.Packets
{
    public class DronePositionPacket : INetSerializable
    {
        public DronePositionData Data;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Data.DroneNetId);
            writer.Put(Data.Thrust);
            writer.PutUnmanaged(Data.Position);
            writer.PutUnmanaged(Data.Rotation);
            writer.PutUnmanaged(Data.Velocity);
            writer.PutUnmanaged(Data.AngularVelocity);
            Plugin.Logger.LogWarning(writer.Length);
        }

        public void Deserialize(NetDataReader reader)
        {
            Data = new DronePositionData
            {
                DroneNetId = reader.GetInt(),
                Thrust = reader.GetFloat(),
                Position = reader.GetUnmanaged<Vector3>(),
                Rotation = reader.GetUnmanaged<Quaternion>(),
                Velocity = reader.GetUnmanaged<Vector3>(),
                AngularVelocity = reader.GetUnmanaged<Vector3>(),
            };
        }
    }
}
