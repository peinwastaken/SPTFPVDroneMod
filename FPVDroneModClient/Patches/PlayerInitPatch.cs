using EFT;
using EFT.CameraControl;
using FPVDroneModClient.Components.Gear;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModClient.Patches
{
    public class PlayerInitPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player), nameof(Player.Init));
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance)
        {
            if (__instance.IsYourPlayer)
            {
                DroneEquipmentObserver observer = __instance.gameObject.AddComponent<DroneEquipmentObserver>();
            }
        }
    }
}
