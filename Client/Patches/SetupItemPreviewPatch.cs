#if !UNITY_EDITOR
using EFT.UI.WeaponModding;
using FPVDroneMod.Components;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneMod.Patches
{
    public class CreateRotatorPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(WeaponPreview), nameof(WeaponPreview.method_5));
        }

        private static void TryResetItemTransform(WeaponPreview __instance, GameObject itemGameObject)
        {
            DroneController controller = itemGameObject.GetComponentInChildren<DroneController>(true);

            if (controller != null)
            {
                controller.ResetTransform();
            }
        }

        [PatchPostfix]
        public static void PatchPostfix(WeaponPreview __instance, GameObject itemGameObject)
        {
            TryResetItemTransform(__instance, itemGameObject);
        }
    }
}
#endif
