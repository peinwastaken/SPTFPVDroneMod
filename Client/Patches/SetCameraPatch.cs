#if !UNITY_EDITOR
using FPVDroneMod.Helpers;
using FPVDroneMod.Components;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace FPVDroneMod.Patches
{
    public class SetCameraPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(CameraClass), nameof(CameraClass.SetCamera));
        }
        
        [PatchPostfix]
        private static void PatchPostfix(CameraClass __instance)
        {
            FPVScreenPostProcess fpvPostProcess = __instance.Camera.gameObject.AddComponent<FPVScreenPostProcess>();
            fpvPostProcess.enabled = false;
            fpvPostProcess.blurMat = AssetHelper.BlurMaterial;
            fpvPostProcess.analogMat = AssetHelper.AnalogMaterial;
            fpvPostProcess.noiseMat = AssetHelper.NoiseMaterial;
            fpvPostProcess.scanMat = AssetHelper.ScanMaterial;
        }
    }
}
#endif