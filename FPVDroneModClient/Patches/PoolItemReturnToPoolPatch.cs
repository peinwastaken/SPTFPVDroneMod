using EFT.AssetsManager;
using EFT.UI;
using FPVDroneModClient.Components.Base;
using FPVDroneModClient.Components.Drone;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModClient.Patches
{
    public class PoolItemReturnToPoolPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(AssetPoolObject), nameof(AssetPoolObject.ReturnToPool));
        }

        [PatchPrefix]
        private static void PatchPrefix(AssetPoolObject __instance)
        {
            BaseDroneController droneController = __instance.gameObject.GetComponent<BaseDroneController>();

            if (droneController && droneController is FPVDroneController controller)
            {
                if (controller.Armable?.IsArmed == true)
                {
                    controller.Detonatable.Detonate();
                }
            }
        }

        [PatchPostfix]
        private static void PatchPostfix(AssetPoolObject __instance)
        {
            BaseDroneController droneController = __instance.gameObject.GetComponent<BaseDroneController>();

            if (droneController)
            {
                if (droneController == DroneHelper.CurrentController)
                {
                    DroneHelper.CurrentController = null;
                }

                droneController.IsInInventory = true;
                droneController.WasJustDropped = false;
            }
        }
    }
}

