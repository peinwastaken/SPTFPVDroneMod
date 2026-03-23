using Fika.Core.Networking.LiteNetLib.Utils;
using FPVDroneModFika.Data;
using UnityEngine;

namespace FPVDroneModFika.Packets
{
    public class DroneExplosionPacket : INetSerializable
    {
        public DroneExplosionData Data;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.PutUnmanaged(Data.Position);
            writer.Put(Data.Damage);
            writer.Put(Data.MaxDistance);
            writer.Put(Data.HeavyBleedDelta);
            writer.Put(Data.LightBleedDelta);
            writer.Put(Data.FractureDelta);
            writer.Put(Data.StaminaBurnRate);
        }

        public void Deserialize(NetDataReader reader)
        {
            Data = new DroneExplosionData
            {
                Position = reader.GetUnmanaged<Vector3>(),
                Damage = reader.GetFloat(),
                MaxDistance = reader.GetFloat(),
                HeavyBleedDelta = reader.GetFloat(),
                LightBleedDelta = reader.GetFloat(),
                FractureDelta = reader.GetFloat(),
                StaminaBurnRate = reader.GetFloat()
            };
        }
    }
}
