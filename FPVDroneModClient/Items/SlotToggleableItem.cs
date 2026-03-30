using EFT.InventoryLogic;
using FPVDroneModClient.Components;
using FPVDroneModClient.Interface;

namespace FPVDroneModClient.Items
{
    public abstract class SlotToggleableItem(string id, ItemTemplate template) : Item(id, template)
    {
        public SlotVisibilityToggler SlotToggleController { get; set; }
        public Item Item;
            
        public virtual void OnItemEquipped(bool equipped)
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
