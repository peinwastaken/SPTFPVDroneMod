#if !UNITY_EDITOR
using EFT;
using EFT.CameraControl;
using FPVDroneModClient.Components.Gear;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModClient.Patches
{
    public class CameraControllerAwakePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PlayerCameraController), nameof(PlayerCameraController.Construct));
        }

        [PatchPostfix]
        private static void PatchPostfix(PlayerCameraController __instance)
        {
            Player player = __instance.Player;
            DroneEquipmentObserver observer = player.gameObject.GetComponent<DroneEquipmentObserver>();

            if (observer)
            {
                observer.OnEyeWearChanged();
            }
        }
    }
}
#endif
