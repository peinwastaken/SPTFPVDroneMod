#if !UNITY_EDITOR
using EFT;
using FPVDroneMod.Components;
using FPVDroneMod.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneMod.Patches
{
    public class GameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            // ???
            AssetHelper.LoadAssets();
            AssetHelper.LoadSounds();

            __instance.gameObject.AddComponent<DroneCullingManager>();

            DebugLogger.LogWarning("gameworld started!!");
        }
    }
}
#endif
