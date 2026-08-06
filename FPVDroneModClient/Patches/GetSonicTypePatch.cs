using EFT.InventoryLogic;
using FPVDroneModClient.Items;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModClient.Patches;

public class GetSonicTypePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(AmmoTemplate), nameof(AmmoTemplate.GetCachedSonicType));
    }

    [PatchPrefix]
    private static bool PatchPrefix(AmmoTemplate __instance, ref SonicBulletSoundPlayer.SonicType __result)
    {
        if (__instance is PayloadItemTemplate payload)
        {
            __result = SonicBulletSoundPlayer.SonicType.SonicShotgun;
            return false;
        }

        return true;
    }
}
