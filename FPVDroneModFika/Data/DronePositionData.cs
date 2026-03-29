using UnityEngine;

namespace FPVDroneModFika.Data
{
    public struct DronePositionData
    {
        public int DroneNetId;
        public float Thrust;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        public float Timestamp;
        public float Time;
    }
}
