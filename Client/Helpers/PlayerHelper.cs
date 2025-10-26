#if !UNITY_EDITOR
using EFT;
using EFT.InventoryLogic;
using FPVDroneMod.Globals;
using FPVDroneMod.Patches;
using System;

namespace FPVDroneMod.Helpers
{
    public static class PlayerHelper
    {
        public static Slot GetEquipmentSlotOfType(Type itemType)
        {
            Player localPlayer = InstanceHelper.LocalPlayer;
            InventoryEquipment equipment = localPlayer.Equipment;

            foreach (Slot slot in equipment.Slots)
            {
                Item containedItem = slot.ContainedItem;

                if (containedItem?.GetType() == itemType)
                {
                    return slot;
                }
            }

            return null;
        }
        
        public static Slot GetEquipmentItemOfId(string itemId)
        {
            Player localPlayer = InstanceHelper.LocalPlayer;
            InventoryEquipment equipment = localPlayer.Equipment;

            foreach (Slot slot in equipment.Slots)
            {
                Item containedItem = slot.ContainedItem;

                if (containedItem != null && containedItem.StringTemplateId == itemId)
                {
                    return slot;
                }
            }

            return null;
        }
        
        public static Slot GetEquipmentSlotOfId(string slotId)
        {
            Player localPlayer = InstanceHelper.LocalPlayer;
            InventoryEquipment equipment = localPlayer.Equipment;

            foreach (Slot slot in equipment.Slots)
            {
                if (slot.ID == slotId)
                {
                    return slot;
                }
            }

            return null;
        }

        public static Weapon GetEquippedWeapon()
        {
            return InstanceHelper.LocalPlayer.GetComponent<Player.FirearmController>()?.Weapon;
        }
    }
}
#endif
