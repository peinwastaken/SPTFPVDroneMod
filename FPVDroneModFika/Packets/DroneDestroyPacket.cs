using Fika.Core.Networking.LiteNetLib.Utils;
using FPVDroneModFika.Data;

namespace FPVDroneModFika.Packets
{
    public class DroneDestroyPacket : INetSerializable
    {
        public DroneDestroyData Data;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Data.DroneNetId);
        }

        public void Deserialize(NetDataReader reader)
        {
            Data = new DroneDestroyData
            {
                DroneNetId = reader.GetInt()
            };
        }
    }
}
