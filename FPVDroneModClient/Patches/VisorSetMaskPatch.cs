#if !UNITY_EDITOR
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
            return AccessTools.Method(typeof(VisorEffect), nameof(VisorEffect.SetMask));
        }

        [PatchPostfix]
        private static void PatchPostfix(VisorEffect __instance)
        {
            Slot eyeSlot = InstanceHelper.LocalPlayer.Equipment.GetSlot(EquipmentSlot.Eyewear);
            ArmoredEquipment eyeItem = (ArmoredEquipment)eyeSlot.ContainedItem;

            if (eyeItem != null && eyeItem.StringTemplateId == ItemIds.HeadsetTemplateId)
            {
                Material visorMaterial = __instance.GetMaterial();
                visorMaterial.SetTexture("_Mask", AssetHelper.FpvGogglesMask);
            }
        }
    }
}
#endif