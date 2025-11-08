#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using FPVDroneMod.Components;
using FPVDroneMod.Config;
using UnityEngine;

namespace FPVDroneMod.Helpers
{
    public static class InstanceHelper
    {
        public static Player LocalPlayer => Singleton<GameWorld>.Instance.MainPlayer;
        public static Camera Camera => CameraClass.Instance.Camera;
        public static FPVScreenPostProcess StaticEffect => Camera.GetComponent<FPVScreenPostProcess>();
        public static DroneHudController DroneHudController { get; set; }
        public static Canvas DroneHudCanvas { get; set; }

        public static void UpdatePostProcessFromConfig()
        {
            var pp = StaticEffect;
            if (pp == null)
            {
                Plugin.Logger.LogError("NO POSTPROCESS COMPONENT FOUND");
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
    }
}
#endif