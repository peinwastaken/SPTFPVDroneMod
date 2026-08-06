using EFT.InventoryLogic;
using FPVDroneModClient.Components;

namespace FPVDroneModClient.Interface
{
    public interface ISlotToggleable
    {
        public SlotVisibilityToggler SlotToggleController { get; set; }
        public Item Item { get; set; }

        public void OnItemEquipped(bool equipped)
        {
            if (equipped)
            {
                SlotToggleController.OnEquip();
            }
            else
            {
                SlotToggleController.OnUnequip();
            }
        }
    }
}
