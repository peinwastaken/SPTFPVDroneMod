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

            InstanceHelper.LocalPlayer.OnGlassesChanged -= OnGlassesChanged; 
            InstanceHelper.LocalPlayer.OnGlassesChanged += OnGlassesChanged;

            DebugLogger.LogWarning("gameworld started!!");
        }

        private static void OnGlassesChanged(VisorsItemClass visor, bool glassesFound)
        {
            if (visor.StringTemplateId == ItemIds.HeadsetTemplateId)
            {
                Material material_0 = (Material)AccessTools.Field(typeof(VisorEffect), "material_0").GetValue(CameraClass.Instance.VisorEffect);
                PlayerCameraController c = InstanceHelper.LocalPlayer.GetComponent<PlayerCameraController>();
                c.method_3(visor.FaceShield);
                CameraClass.Instance.VisorEffect.ScratcesIntensity = 0f;
                material_0.SetTexture("_Mask", AssetHelper.FpvGogglesMask);
            }
        }
    }
}
#endif
