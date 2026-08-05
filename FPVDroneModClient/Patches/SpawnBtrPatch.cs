#if !UNITY_EDITOR
using EFT.Vehicle;
using System.Reflection;
using FPVDroneModClient.Config;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace FPVDroneModClient.Patches
{
    public class SpawnBtrPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BtrController), nameof(BtrController.InitServer));
        }

        [PatchPrefix]
        private static bool PatchPrefix(BtrController __instance)
        {
            // if tank is dead and perma death is enabled dont spawn the tank
            if (Plugin.TankDeathState.IsDead &&
                GeneralConfig.EnableTankPermaDeath.Value)
            {
                return false;
            }

            return true;
        }
    }
}
#endif