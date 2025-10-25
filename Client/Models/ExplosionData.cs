using UnityEngine;

namespace FPVDroneMod.Models
{
    public class ExplosionData
    {
        public Vector3 Position = Vector3.zero;
        public float MaxDistance = 5f;
        public float Damage = 200f;
        public string EffectName = "Grenade_new";
        public float FractureDelta = 0.6f;
        public float HeavyBleedDelta = 0.4f;
        public float LightBleedDelta = 0.7f;
        public float StaminaBurnRate = 0.7f;
        public float InstantKillDistance = -1f;
        public Vector3 EffectDirection = Vector3.up;
    }
}
