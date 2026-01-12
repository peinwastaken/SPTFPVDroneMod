#if !UNITY_EDITOR
using EFT;
using FPVDroneModClient.Components;
using FPVDroneModClient.Helpers;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace FPVDroneModClient.Patches
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

            DroneHelper.SelectedControllers = [];
            
            InstanceHelper.CreateHudCamera();
            InstanceHelper.CreatePostProcessCamera();

            DebugLogger.LogWarning("gameworld started!!");
        }
    }
}
#endif
