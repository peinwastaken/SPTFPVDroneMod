using System.Collections;
using FPVDroneModClient.Interface;
using UnityEngine;

namespace FPVDroneModClient.Components.Drone
{
    public class DroneDetonator : MonoBehaviour, IPhysicsTrigger
    {
        public bool Armed;

        private IDetonatable _detonatable;
        public string Description { get; }

        public void OnTriggerEnter(Collider other)
        {
            int layerMask = 1 << other.gameObject.layer;
            bool hitSomethingOrWater = (layerMask & LayersMaskController.TripwireCheckLayerMask) != 0
                                       || (layerMask & LayersMaskController.WaterLayer) != 0;

            if (Armed && hitSomethingOrWater)
            {
                _detonatable.Detonate();
            }
        }

        public void OnTriggerExit(Collider collider)
        {

        }

        private void Start()
        {
            _detonatable = GetComponentInParent<IDetonatable>();
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
            Armed = armed;
        }
    }
}

