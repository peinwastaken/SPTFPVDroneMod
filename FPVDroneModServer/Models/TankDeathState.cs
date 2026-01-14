using System.Numerics;
using System.Text.Json.Serialization;

namespace FPVDroneModServer.Models
{
    public class TankDeathState
    {
        [JsonPropertyName("isDead")]
        public required bool IsDead { get; set; } = false;

        [JsonPropertyName("deathMap")]
        public required string DeathMap { get; set; }
    
        [JsonPropertyName("deathPosition")]
        public required Vector DeathPosition { get; set; }
    
        [JsonPropertyName("deathAngle")]
        public required Vector DeathAngle { get; set; }
    }
}