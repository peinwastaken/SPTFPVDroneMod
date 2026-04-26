#if !UNITY_EDITOR
using EFT.InventoryLogic;
using FPVDroneModClient.Components;

namespace FPVDroneModClient.Items
{
    public abstract class BaseSlotToggleable(string id, ItemTemplate template) : Item(id, template)
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
#endif