#if !UNITY_EDITOR
using EFT;
using EFT.InventoryLogic;
using FPVDroneModClient.Components.Gear;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModClient.Patches
{
    public class PlayerItemAddedOrRemovedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player), nameof(Player.OnItemAddedOrRemoved));
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance, Item item, ItemAddress location)
        {
            DebugLogger.LogWarning($"item added or removed!!!");

            if (item.StringTemplateId == ItemIds.HeadsetTemplateId)
            {
                DroneEquipmentObserver observer = __instance.gameObject.GetComponent<DroneEquipmentObserver>();

                if (observer)
                {
                    observer.OnEyeWearChanged();
                }
            }
        }
    }
}
#endif