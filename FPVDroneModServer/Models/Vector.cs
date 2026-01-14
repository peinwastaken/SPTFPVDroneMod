using System.Text.Json.Serialization;

namespace FPVDroneModServer.Models
{
    public class Vector
    {
        [JsonPropertyName("x")]
        public float X { get; set; } = 0f;
    
        [JsonPropertyName("y")]
        public float Y { get; set; } = 0f;
    
        [JsonPropertyName("z")]
        public float Z { get; set; } = 0f;
    }
}