using System;
using UnityEngine;

namespace FPVDroneModClient.Components
{
    public class Detonator : MonoBehaviour, IPhysicsTrigger
    {
        public string Description { get; } 

        public event Action<Collider> TriggerEntered;
        public event Action<Collider> TriggerExited;

        public virtual void OnTriggerEnter(Collider collider)
        {
            TriggerEntered?.Invoke(collider);
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            TriggerEntered?.Invoke(collider);
        }
    }
}
