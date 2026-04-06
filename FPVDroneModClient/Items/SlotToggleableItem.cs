using EFT.InventoryLogic;
using FPVDroneModClient.Components;
using FPVDroneModClient.Interface;

namespace FPVDroneModClient.Items
{
    public abstract class SlotToggleableItem<T>(string id, ItemTemplate template) : BaseSlotToggleable(id, template)
    {
    }
}
