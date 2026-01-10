using FPVDroneModClient.Interface;
using System.Collections;
using UnityEngine;

namespace FPVDroneModClient.Components
{
    public class DroneDetonator : MonoBehaviour, IPhysicsTrigger
    {
        public bool Armed;

        private IDetonatable _detonatable;
        public string Description { get; }

        public void OnTriggerEnter(Collider other)
        {
            #if !UNITY_EDITOR
            int layerMask = 1 << other.gameObject.layer;
            bool hitSomethingOrWater = (layerMask & LayerMaskClass.TripwireCheckLayerMask) != 0
                                       || (layerMask & LayerMaskClass.WaterLayer) != 0;

            if (Armed && hitSomethingOrWater)
            {
                _detonatable?.Detonate();
            }
            #endif
        }

        public void OnTriggerExit(Collider collider)
        {

        }

        #if !UNITY_EDITOR
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
        #endif
    }
}
