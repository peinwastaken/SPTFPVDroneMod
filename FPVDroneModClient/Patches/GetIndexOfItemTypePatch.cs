#if !UNITY_EDITOR
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace FPVDroneModClient.Patches
{
    public class GetIndexOfItemTypePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ItemExtensions), nameof(ItemExtensions.GetIndexOfItemType));
        }

        [PatchPrefix]
        public static bool PatchPrefix(ref int __result, Item i)
        {
            Type type = i.GetType();
            
            int index = ItemSorter.IndexOf(type);
            if (index >= 0)
            {
                __result = index;
                return false;
            }

            for (Type type2 = type; type2 != null; type2 = type2.BaseType)
            {
                index = ItemSorter.IndexOf(type2);
                if (index >= 0)
                {
                    __result = index;
                    return false;
                }
            }

            __result = int.MaxValue;
            return false;
        }
    }
}
#endif
