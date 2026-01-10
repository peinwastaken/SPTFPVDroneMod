#if !UNITY_EDITOR
using EFT;
using FPVDroneMod.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneMod.Patches
{
    public class LocalPlayerDiedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player), nameof(Player.OnDead));
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance)
        {
            if (__instance.IsYourPlayer)
            {
                DroneHelper.ControlDrone(false);
            }
        }
    }
}
#endif