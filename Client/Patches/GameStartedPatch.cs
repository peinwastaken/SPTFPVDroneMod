#if !UNITY_EDITOR
using EFT;
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
        private static void PatchPostfix()
        {
            // ???
            AssetHelper.LoadAssets();
            AssetHelper.LoadSounds();
            
            DebugLogger.LogWarning("gameworld started!!");
        }
    }
}
#endif
