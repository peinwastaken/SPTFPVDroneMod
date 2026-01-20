using FPVDroneModClient.Components.Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPVDroneModClient.Components
{
    public class ElectronicWarfareController : MonoBehaviour
    {
        public List<BaseDroneController> DronesInRange = [];
        
        public float MaxRange = 150f;
        public float MinRange = 10f;

        private float _timeSinceLastCheck = 0f;
        private Collider[] _colliders = new Collider[16];
        private readonly float _timeBetweenChecks = 0.1f;

        private void Update()
        {
            _timeSinceLastCheck += Time.deltaTime;

            if (_timeSinceLastCheck >= _timeBetweenChecks)
            {
                GetDronesInRange();
                _timeSinceLastCheck = 0f;
            }
        }

        private void GetDronesInRange()
        {
            // remove out of range drones
            for (int i = DronesInRange.Count - 1; i >= 0; i--)
            {
                BaseDroneController controller = DronesInRange[i];
                if (!controller || (controller.transform.position - transform.position).magnitude > MaxRange)
                {
                    DronesInRange.RemoveAt(i);
                }
            }
            
            // get drones in range
            int droneCount = Physics.OverlapSphereNonAlloc(transform.position, MaxRange, _colliders, LayerMaskClass.DeadbodyLayer);
            
            for (int i = 0; i < droneCount; i++)
            {
                Collider collider = _colliders[i];
                BaseDroneController controller = collider.GetComponentInParent<BaseDroneController>();
                
                if (controller && !DronesInRange.Contains(controller))
                {
                    DronesInRange.Add(controller);
                }
            }
        }

        private void Awake()
        {
            ElectronicWarfareManager.Instance?.AddController(this);
        }

        private void OnDestroy()
        {
            ElectronicWarfareManager.Instance?.RemoveController(this);
        }
    }
}
