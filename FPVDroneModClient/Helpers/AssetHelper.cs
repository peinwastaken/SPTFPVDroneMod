#if !UNITY_EDITOR
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace FPVDroneModClient.Helpers
{
    public static class AssetHelper
    {
        public static Material BlurMaterial;
        public static Material NoiseMaterial;
        public static Material AnalogMaterial;
        public static Material ScanMaterial;
        public static AudioClip DroneAudioClip;
        public static Texture DroneNightVisionLens;
        public static Texture DroneNightVisionMask;

        public static string AssemblyDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string AssetDirPath => Path.Combine(AssemblyDir, "assets");
        public static string BundleDirPath => Path.Combine(AssetDirPath, "bundles");
        public static string SoundDirPath => Path.Combine(AssetDirPath, "sounds");
        public static string TextureDirPath => Path.Combine(AssetDirPath, "textures");
        
        public static string ShadersBundlePath => Path.Combine(BundleDirPath, "drone_shaders_new.bundle");
        
        public static AssetBundle ShadersBundle;
        
        public static async Task<AudioClip> LoadAudioClip(string path, string fileName)
        {
            string finalPath = Path.Combine(path, fileName);

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file://{finalPath}", AudioType.WAV))
            {
                UnityWebRequestAsyncOperation request =  www.SendWebRequest();
                while (!request.isDone) await Task.Yield();

                if (www.isHttpError || www.isNetworkError)
                {
                    DebugLogger.LogError($"Error loading audio clip from {fileName}: {www.error}");
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    clip.name = fileName;
                    
                    DebugLogger.LogInfo($"Loaded audio clip: {clip.name}");

                    return clip;
                }
            }

            return null;
        }
        
        public static Texture LoadTexture(string filePath, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(File.ReadAllBytes(filePath));
            tex.wrapMode = wrapMode;
            
            return tex;
        }

        public static void LoadBundles()
        {
            AssetBundle shaderBundle = AssetBundle.LoadFromFile(ShadersBundlePath);
            ShadersBundle = shaderBundle;
        }

        public static async void LoadSounds()
        {
            DroneAudioClip = await LoadAudioClip(SoundDirPath, "drone_sound_loop.wav");
            
            DebugLogger.LogInfo("Loaded sounds!");
        }
        
        public static void LoadAssets()
        {
            BlurMaterial = ShadersBundle.LoadAsset<Material>("assets/drone/shaders/BlurMaterial.mat");
            NoiseMaterial = ShadersBundle.LoadAsset<Material>("assets/drone/shaders/NoiseMaterial.mat");
            AnalogMaterial = ShadersBundle.LoadAsset<Material>("assets/drone/shaders/AnalogMaterial.mat");
            ScanMaterial = ShadersBundle.LoadAsset<Material>("assets/drone/shaders/ScanlinesMaterial.mat");
            
            DroneNightVisionLens = LoadTexture(Path.Combine(TextureDirPath, "nvglens.png"));
            DroneNightVisionMask = LoadTexture(Path.Combine(TextureDirPath, "nvgmask.png"));
            
            DebugLogger.LogInfo("Loaded assets!");
        }
    }
}
#endif
