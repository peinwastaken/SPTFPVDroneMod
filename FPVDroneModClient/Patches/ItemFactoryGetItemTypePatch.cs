#if !UNITY_EDITOR
using EFT.InventoryLogic;
using EFT.UI.DragAndDrop;
using FPVDroneModClient.Items;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace FPVDroneModClient.Patches
{
    public class ItemFactoryGetItemTypePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ItemViewFactory), nameof(ItemViewFactory.GetSpecialIcon));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref string __result, Item item)
        {
            if (item is DroneItem)
            {
                __result = "special_item";
                return false;
            }

            return true;
        }
    }
}
#endif