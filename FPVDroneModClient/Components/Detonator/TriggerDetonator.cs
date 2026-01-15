using FPVDroneModClient.Components.Base;

namespace FPVDroneModClient.Components.Detonator
{
    public class TriggerDetonator : BasePayloadController
    {
        public override void ToggleArmed()
        {
            Detonate(gameObject.transform.position, null, null);
        }
    }
}