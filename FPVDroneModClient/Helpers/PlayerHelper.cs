using EFT;
using EFT.Communications;
using EFT.InventoryLogic;
using System;

namespace FPVDroneModClient.Helpers
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

        public static void ClearInteractions(this Player player)
        {
            EftGamePlayerOwner playerOwner = player.GetComponent<EftGamePlayerOwner>();
            playerOwner?.ClearInteractionState();
        }

        public static bool IsFriendlyToBot(this IPlayer player, BotOwner botOwner)
        {
            if (player == null) return false;

            if (!botOwner.EnemiesController.IsEnemy(player))
            {
                return true;
            }

            BotsGroup group = botOwner.BotsGroup;
            if (group.Allies.Contains(player) && !group.IsPlayerEnemy(player) && group.Neutrals.ContainsKey(player))
            {
                return true;
            }
            
            return false;
        }
    }
}

