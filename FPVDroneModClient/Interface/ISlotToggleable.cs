using FPVDroneModClient.Components;

namespace FPVDroneModClient.Interface
{
    public interface ISlotToggleable
    {
        public SlotVisibilityToggler SlotToggleController { get; set; }

        public void OnItemEquipped(bool equipped);
    }
}
