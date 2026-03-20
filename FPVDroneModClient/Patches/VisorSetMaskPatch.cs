using EFT;
using EFT.InventoryLogic;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneModClient.Patches
{
    public class VisorSetMaskPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player), nameof(Player.OnItemAddedOrRemoved));
        }

        /*
        [PatchPostfix]
        private static void PatchPostfix(Player __instance, Item item, ItemAddress location, bool added)
        {
            string itemId = item.StringTemplateId;

            if (itemId == ItemIds.HeadsetTemplateId && location.)
            {
                
            }
        }*/
    }
}
