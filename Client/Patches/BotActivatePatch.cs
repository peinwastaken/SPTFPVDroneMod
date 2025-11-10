using EFT;
using FPVDroneMod.Components;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneMod.Patches
{
    public class BotActivatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BotOwner), nameof(BotOwner.method_10));
        }

        [PatchPostfix]
        private static void PatchPostfix(BotOwner __instance)
        {
            __instance.GetPlayer.gameObject.AddComponent<BotDroneListener>();
        }
    }
}
