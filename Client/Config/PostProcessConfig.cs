#if !UNITY_EDITOR
using BepInEx.Configuration;
using FPVDroneMod.Globals;
using FPVDroneMod.Helpers;

namespace FPVDroneMod.Config
{
    public static class PostProcessConfig
    {
        // noise overlay settings
        public static ConfigEntry<float> NoiseIntensity;
        public static ConfigEntry<int> NoiseResX;
        public static ConfigEntry<int> NoiseResY;

        // screen blur settings
        public static ConfigEntry<float> BlurSize;

        // analog shader settings
        public static ConfigEntry<float> AnalogChromatic;
        public static ConfigEntry<float> AnalogDesaturation;
        public static ConfigEntry<int> AnalogPosterizeLevels;
        public static ConfigEntry<float> AnalogSepiaStrength;

        public static void Bind(int order, string category, ConfigFile cfg)
        {
            string formatted = Category.Format(order, category);

            // noise overlay settings
            NoiseIntensity = cfg.Bind(formatted, "Noise Intensity", 0.093f, new ConfigDescription(
                "controls the intensity of the noise overlay",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes() { Order = 1000 }));

            NoiseResX = cfg.Bind(formatted, "Noise Resolution X", 102, new ConfigDescription(
                "horizontal noise texture resolution",
                new AcceptableValueRange<int>(1, 2048),
                new ConfigurationManagerAttributes() { Order = 990 }));

            NoiseResY = cfg.Bind(formatted, "Noise Resolution Y", 512, new ConfigDescription(
                "vertical noise texture resolution",
                new AcceptableValueRange<int>(1, 2048),
                new ConfigurationManagerAttributes() { Order = 980 }));


            // screen blur settings
            BlurSize = cfg.Bind(formatted, "Screen Blur Size", 1.5f, new ConfigDescription(
                "blur radius applied to the screen",
                new AcceptableValueRange<float>(0f, 10f),
                new ConfigurationManagerAttributes() { Order = 970 }));


            // analog shader settings
            AnalogChromatic = cfg.Bind(formatted, "Analog Chromatic Aberration", 0.0024f, new ConfigDescription(
                "strength of the chromatic aberration effect",
                new AcceptableValueRange<float>(0f, 0.01f),
                new ConfigurationManagerAttributes() { Order = 960 }));

            AnalogDesaturation = cfg.Bind(formatted, "Analog Desaturation", 0.081f, new ConfigDescription(
                "how much color desaturation to apply",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes() { Order = 950 }));

            AnalogPosterizeLevels = cfg.Bind(formatted, "Analog Posterize Levels", 64, new ConfigDescription(
                "color quantization levels for analog posterization",
                new AcceptableValueRange<int>(1, 256),
                new ConfigurationManagerAttributes() { Order = 940 }));

            AnalogSepiaStrength = cfg.Bind(formatted, "Analog Sepia Strength", 0.155f, new ConfigDescription(
                "strength of the sepia tone effect",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes() { Order = 930 }));
            
            NoiseIntensity.SettingChanged += (_, __) => InstanceHelper.UpdatePostProcessFromConfig();
            NoiseResX.SettingChanged += (_, __) => InstanceHelper.UpdatePostProcessFromConfig();
            NoiseResY.SettingChanged += (_, __) => InstanceHelper.UpdatePostProcessFromConfig();
            BlurSize.SettingChanged += (_, __) => InstanceHelper.UpdatePostProcessFromConfig();
            AnalogChromatic.SettingChanged += (_, __) => InstanceHelper.UpdatePostProcessFromConfig();
            AnalogDesaturation.SettingChanged += (_, __) => InstanceHelper.UpdatePostProcessFromConfig();
            AnalogPosterizeLevels.SettingChanged += (_, __) => InstanceHelper.UpdatePostProcessFromConfig();
            AnalogSepiaStrength.SettingChanged += (_, __) => InstanceHelper.UpdatePostProcessFromConfig();
        }
    }
}
#endif
