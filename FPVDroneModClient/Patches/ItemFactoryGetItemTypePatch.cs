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
            return AccessTools.Method(typeof(ItemViewFactory), nameof(ItemViewFactory.GetItemType));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref EItemType __result, Type itemType)
        {
            if (itemType == typeof(DroneItem))
            {
                __result = EItemType.Special;
                return false;
            }

            return true;
        }
    }
}
