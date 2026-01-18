using FPVDroneModClient.Components.Base;
using UnityEngine;

namespace FPVDroneModClient.Components.Payloads
{
    public class TriggerPayload : BasePayloadController
    {
        public override void ToggleArmed()
        {
            Detonate();
        }

        public override void OnTriggerEnter(Collider collider)
        {
            
        }

        public override void OnTriggerExit(Collider collider)
        {
            
        }
    }
}