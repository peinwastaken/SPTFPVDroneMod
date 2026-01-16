#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using System.Reflection;
using FPVDroneModClient.Config;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace FPVDroneModClient.Patches;

public class SpawnBtrPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BTRControllerClass), nameof(BTRControllerClass.method_7));
    }

    [PatchPrefix]
    private static bool PatchPrefix(BTRControllerClass __instance)
    {
        if (Plugin.TankDeathState.IsDead && GeneralConfig.EnableTankPermaDeath.Value && Singleton<GameWorld>.Instance.LocationId == Plugin.TankDeathState.DeathMap)
        {
            return false;
        }

        return true;
    }
}
#endif