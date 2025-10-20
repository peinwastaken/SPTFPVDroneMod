#if !UNITY_EDITOR
using FPVDroneMod.Components;
using FPVDroneMod.Patches;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace FPVDroneMod.Helpers
{
    public static class AssetHelper
    {
        public static Material BlurMaterial;
        public static Material NoiseMaterial;
        public static Material AnalogMaterial;
        public static Material ScanMaterial;
        public static GameObject HUDPrefab;
        public static AudioClip DroneAudioClip;

        public static string AssemblyDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string AssetDirPath => Path.Combine(AssemblyDir, "assets");
        public static string BundleDirPath => Path.Combine(AssetDirPath, "bundles");
        public static string SoundDirPath => Path.Combine(AssetDirPath, "sounds");
        
        public static string ShadersBundlePath => Path.Combine(BundleDirPath, "drone_shaders_new.bundle");
        public static string UIBundlePath => Path.Combine(BundleDirPath, "drone_hud.bundle");
        
        public static async Task<AudioClip> LoadAudioClip(string path, string fileName)
        {
            string finalPath = Path.Combine(path, fileName);

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file://{finalPath}", AudioType.WAV))
            {
                UnityWebRequestAsyncOperation request =  www.SendWebRequest();
                while (!request.isDone) await Task.Yield();

                if (www.isHttpError || www.isNetworkError)
                {
                    Plugin.Logger.LogError($"Error loading audio clip from {fileName}: {www.error}");
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    clip.name = fileName;

                    return clip;
                }
            }

            return null;
        }

        public static async void LoadSounds()
        {
            DroneAudioClip = await LoadAudioClip(SoundDirPath, "drone_sound_loop.wav");
        }
        
        public static void LoadAssets()
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(ShadersBundlePath);

            BlurMaterial = bundle.LoadAsset<Material>("assets/drone/shaders/BlurMaterial.mat");
            NoiseMaterial = bundle.LoadAsset<Material>("assets/drone/shaders/NoiseMaterial.mat");
            AnalogMaterial = bundle.LoadAsset<Material>("assets/drone/shaders/AnalogMaterial.mat");
            ScanMaterial = bundle.LoadAsset<Material>("assets/drone/shaders/ScanlinesMaterial.mat");
            
            AssetBundle uiBundle = AssetBundle.LoadFromFile(UIBundlePath);
            
            HUDPrefab = uiBundle.LoadAsset<GameObject>("assets/drone/ui/DroneHud.prefab");
            
            GameObject droneHud = GameObject.Instantiate(HUDPrefab);
            GameObject.DontDestroyOnLoad(droneHud);
            InstanceHelper.DroneHudController = droneHud.GetComponent<DroneHudController>();
            
            Canvas canvas = droneHud.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.planeDistance = 0.055f;
            InstanceHelper.DroneHudCanvas = canvas;
            
            droneHud.SetActive(false);
        }
    }
}
#endif
