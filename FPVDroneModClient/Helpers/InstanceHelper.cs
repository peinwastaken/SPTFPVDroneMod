#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using EFT.CameraControl;
using FPVDroneModClient.Components;
using FPVDroneModClient.Config;
using UnityEngine;
using WTTClientCommonLib.Services;

namespace FPVDroneModClient.Helpers
{
    public static class InstanceHelper
    {
        public static Player LocalPlayer => Singleton<GameWorld>.Instance.MainPlayer;
        public static FPVScreenPostProcess StaticEffect => PostProcessCamera?.GetComponent<FPVScreenPostProcess>();
        public static Camera HudCamera { get; set; }
        public static Camera PostProcessCamera { get; set; }
        public static GameObject BtrRecent { get; set; }
        public static GameObject BtrOld { get; set; }

        public static void CreateHudCamera()
        {
            if (HudCamera != null) return;

            GameObject go = new GameObject("DroneHudCamera");
            GameObject.DontDestroyOnLoad(go);
            
            HudCamera = go.AddComponent<Camera>();
            HudCamera.enabled = false;
            HudCamera.clearFlags = CameraClearFlags.Depth;
            HudCamera.cullingMask = LayerMask.GetMask("UI");
            HudCamera.orthographic = true;
            HudCamera.depth = 100f;
            HudCamera.rect = new Rect(0f, 0f, 0.999f, 0.999f);
            HudCamera.nearClipPlane = 0.01f;
            HudCamera.farClipPlane = 10f;
            HudCamera.allowHDR = false;
            HudCamera.allowMSAA = false;
        }
        
        public static void CreatePostProcessCamera()
        {
            if (PostProcessCamera != null) return;

            GameObject go = new GameObject("DronePostProcessCamera");
            GameObject.DontDestroyOnLoad(go);
            
            PostProcessCamera = go.AddComponent<Camera>();
            PostProcessCamera.enabled = false;
            PostProcessCamera.clearFlags = CameraClearFlags.Nothing;
            PostProcessCamera.cullingMask = 0;
            PostProcessCamera.depth = 200f;
            PostProcessCamera.nearClipPlane = 0.01f;
            PostProcessCamera.farClipPlane = 10f;
            PostProcessCamera.allowHDR = false;
            PostProcessCamera.allowMSAA = false;
            
            FPVScreenPostProcess fpvPostProcess = PostProcessCamera.gameObject.AddComponent<FPVScreenPostProcess>();
            fpvPostProcess.enabled = false;
            fpvPostProcess.blurMat = AssetHelper.BlurMaterial;
            fpvPostProcess.analogMat = AssetHelper.AnalogMaterial;
            fpvPostProcess.noiseMat = AssetHelper.NoiseMaterial;
            fpvPostProcess.scanMat = AssetHelper.ScanMaterial;
            UpdatePostProcessFromConfig();
        }

        public static void UpdatePostProcessFromConfig()
        {
            var pp = StaticEffect;
            if (pp == null)
            {
                DebugLogger.LogError("NO POSTPROCESS COMPONENT FOUND");
                return;
            }
            
            if (pp.noiseMat != null)
            {
                pp.noiseMat.SetFloat("_Intensity", PostProcessConfig.NoiseIntensity.Value);
                pp.noiseMat.SetFloat("_ResX", PostProcessConfig.NoiseResX.Value);
                pp.noiseMat.SetFloat("_ResY", PostProcessConfig.NoiseResY.Value);
            }
            
            if (pp.blurMat != null)
            {
                pp.blurMat.SetFloat("_BlurSize", PostProcessConfig.BlurSize.Value);
            }
            
            if (pp.analogMat != null)
            {
                pp.analogMat.SetFloat("_Chromatic", PostProcessConfig.AnalogChromatic.Value);
                pp.analogMat.SetFloat("_Desaturation", PostProcessConfig.AnalogDesaturation.Value);
                pp.analogMat.SetFloat("_PosterizeLevels", PostProcessConfig.AnalogPosterizeLevels.Value);
                pp.analogMat.SetFloat("_SepiaStrength", PostProcessConfig.AnalogSepiaStrength.Value);
            }
        }

        public static void LoadTankAssets()
        {
            AssetLoader assetLoader = WTTClientCommonLib.WTTClientCommonLib.Instance.AssetLoader;
            
            if (!BtrRecent)
            {
                BtrRecent = assetLoader.LoadPrefabFromBundle("btr_destroyed_recent", "Assets/Drone/BTR/Prefabs/btr_destroyed_recent.prefab");
            }

            if (!BtrOld)
            {
                BtrOld = assetLoader.LoadPrefabFromBundle("btr_destroyed_old", "Assets/Drone/BTR/Prefabs/btr_destroyed_old.prefab");
            }
        }

        public static void CreateTankCorpse(Vector3 pos, Vector3 euler, bool wasJustDestroyed)
        {
            GameObject tank = wasJustDestroyed ? BtrRecent : BtrOld;
            GameObject go = GameObject.Instantiate(tank);
            go.transform.position = pos;
            go.transform.eulerAngles = euler;
            go.SetActive(true);
            go.transform.Rotate(go.transform.up, 180f);
        }

        public static void UpdateNoiseAmount(float amount)
        {
            var pp = StaticEffect;

            if (pp)
            {
                pp.noiseMat.SetFloat("_Intensity", Mathf.Clamp(amount, 0.1f, 1f));
            }
        }
    }
}
#endif