using System;
using UnityEngine;

namespace FPVDroneModClient.Components.Drone
{
    public class DroneSignalController : MonoBehaviour
    {
        public bool HasSignal = true;
        public float SignalStrength = 1f;
        
        public float JammingStrength = 0f; // 0 -> 1
        public float DistanceStrength = 1f; // 1 -> 0

        public Action OnSignalLost;
        public Action OnSignalGained;

        private bool _prevSignal;
        
        private void FixedUpdate()
        {
            SignalStrength = DistanceStrength - JammingStrength;
            HasSignal = SignalStrength > 0f;

            if (_prevSignal != HasSignal)
            {
                if (HasSignal)
                {
                    OnSignalGained?.Invoke();
                }
                else
                {
                    OnSignalLost?.Invoke();
                }
            }
            
            _prevSignal = HasSignal;
        }

        private void Update()
        {
            JammingStrength = Mathf.Clamp01(JammingStrength);
            DistanceStrength = Mathf.Clamp01(DistanceStrength);
            SignalStrength = Mathf.Clamp01(SignalStrength);
        }
    }
}
