using System.Reflection;
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
        if (Plugin.TankDeathState.IsDead)
        {
            return false;
        }

        return true;
    }
}