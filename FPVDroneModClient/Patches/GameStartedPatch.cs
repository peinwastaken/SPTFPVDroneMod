#if !UNITY_EDITOR
using EFT;
using FPVDroneModClient.Components;
using FPVDroneModClient.Config;
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

            __instance.gameObject.AddComponent<ElectronicWarfareManager>();
            
            GameObject go = new GameObject("ElectronicWarfare");
            go.AddComponent<ElectronicWarfareController>();
            go.transform.position = Vector3.zero;
            GameObject.DontDestroyOnLoad(go);

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
