#if !UNITY_EDITOR
using EFT;
using EFT.CameraControl;
using FPVDroneModClient.Components;
using FPVDroneModClient.Components.Jamming;
using FPVDroneModClient.Config;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Models;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

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

            __instance.gameObject.AddComponent<DroneCullingManager>();

            DroneHelper.SelectedControllers = [];
            
            InstanceHelper.CreateHudCamera();
            InstanceHelper.CreatePostProcessCamera();
            InstanceHelper.LoadTankAssets();
            
            Camera camera = CameraClass.Instance.Camera;
            DroneHelper.LastFov = camera.fieldOfView;
            DroneHelper.LastNearClip = camera.nearClipPlane;

            TankDeathState deathState = Plugin.TankDeathState;
            if (GeneralConfig.EnableTankPermaDeath.Value && deathState.IsDead && __instance.LocationId == deathState.DeathMap)
            {
                InstanceHelper.CreateTankCorpse(deathState.DeathPosition, deathState.DeathAngle, false);
            }

            DebugLogger.LogWarning("gameworld started!!");
        }
    }
}
#endif
