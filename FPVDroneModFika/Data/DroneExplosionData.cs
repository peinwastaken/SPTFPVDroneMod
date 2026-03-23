using FPVDroneModClient.Models;
using UnityEngine;

namespace FPVDroneModFika.Data
{
    public struct DroneExplosionData
    {
        public Vector3 Position;
        public float Damage;
        public float MaxDistance;
        public float HeavyBleedDelta;
        public float LightBleedDelta;
        public float FractureDelta;
        public float StaminaBurnRate;
    }
}
