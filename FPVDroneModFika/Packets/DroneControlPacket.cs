using Fika.Core.Networking.LiteNetLib.Utils;
using FPVDroneModFika.Data;

namespace FPVDroneModFika.Packets
{
    public class DroneControlPacket : INetSerializable
    {
        public DroneControlData Data;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Data.DroneNetId);
            writer.Put(Data.IsDroneControlled);
        }

        public void Deserialize(NetDataReader reader)
        {
            Data = new DroneControlData
            {
                DroneNetId = reader.GetInt(),
                IsDroneControlled = reader.GetBool()
            };
        }
    }
}
