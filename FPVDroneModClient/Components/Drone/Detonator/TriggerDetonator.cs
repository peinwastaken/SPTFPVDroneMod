using FPVDroneModClient.Components.Base;

namespace FPVDroneModClient.Components.Drone.Detonator
{
    public class TriggerDetonator : BasePayloadController
    {
        public override void ToggleArmed()
        {
            Detonate(gameObject.transform.position, null, null);
        }
    }
}