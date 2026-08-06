using System;
using Newtonsoft.Json;
using UnityEngine;

namespace FPVDroneModClient.Models
{
    public class TankDeathState
    {
        [JsonProperty("isDead")]
        public bool IsDead { get; set; }
        
        [JsonProperty("deathMap")]
        public string DeathMap { get; set; }
        
        [JsonProperty("deathPosition")]
        public Vector3 DeathPosition { get; set; }
        
        [JsonProperty("deathAngle")]
        public Vector3 DeathAngle { get; set; }
    }
}

