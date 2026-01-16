#if !UNITY_EDITOR
using EFT.InventoryLogic;

namespace FPVDroneModClient.Helpers
{
    public static class ItemHelper
    {
        public static Slot GetSlotById(this CompoundItem item, string slotId)
        {
            foreach (Slot slot in item.Slots)
            {
                if (slot.ID == slotId)
                {
                    return slot;
                }
            }

            return null;
        }
    }
}
#endif