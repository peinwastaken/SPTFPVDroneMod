using FPVDroneModClient.Components.Base;
using UnityEngine;

namespace FPVDroneModClient.Components.Drone.Detonator
{
    public class ContactDetonator : BasePayloadController
    {
        public override void OnTriggerEnter(Collider collider)
        {
            int layerMask = 1 << collider.gameObject.layer;
            bool hitSomethingOrWater = (layerMask & LayerMaskClass.TripwireCheckLayerMask) != 0 || (layerMask & LayerMaskClass.WaterLayer) != 0;
        
            if (IsArmed && hitSomethingOrWater)
            {
                Detonate(gameObject.transform.position, null, null);
            }
        }
    }
}