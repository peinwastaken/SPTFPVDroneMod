using System.Collections;
using UnityEngine;

namespace FPVDroneMod.Components
{
    public class DroneDetonator : MonoBehaviour
    {
        public bool Armed = false;
    
        DroneController _droneController;
    
        private void Start()
        {
            _droneController = GetComponentInParent<DroneController>();
        }

        private IEnumerator SetArmedAfterDelay(bool armed, float delay)
        {
            yield return new WaitForSeconds(delay);
        
            SetArmed(armed);
        }

        public void SetArmedDelay(bool armed, float delay)
        {
            StartCoroutine(SetArmedAfterDelay(armed, delay));
        }

        public void SetArmed(bool armed)
        {
            Debug.Log($"Drone Armed Status: {armed}");
            Armed = armed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Armed)
            {
                _droneController.Detonate();
            }
        }
    }
}
