using Fika.Core.Networking.LiteNetLib.Utils;
using FPVDroneModFika.Data;
using UnityEngine;

namespace FPVDroneModFika.Packets
{
    public class TankDestroyPacket : INetSerializable
    {
        public TankDestroyData Data;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.PutUnmanaged(Data.Position);
            writer.PutUnmanaged(Data.EulerAngles);
        }

        public void Deserialize(NetDataReader reader)
        {
            Data = new TankDestroyData
            {
                Position = reader.GetUnmanaged<Vector3>(),
                EulerAngles = reader.GetUnmanaged<Vector3>()
            };
        }
    }
}
