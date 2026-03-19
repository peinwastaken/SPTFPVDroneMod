#if !UNITY_EDITOR
using BepInEx.Configuration;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;

namespace FPVDroneModClient.Config
{
    public static class GeneralConfig
    {
        public static ConfigEntry<bool> EnableDebug;
        public static ConfigEntry<bool> DisableCulling;
        public static ConfigEntry<bool> EnableTankPermaDeath;

        public static void Bind(int order, string category, ConfigFile cfg)
        {
            string formatted = Category.Format(order, category);

            EnableDebug = cfg.Bind(formatted, "Enable Debug Logging", false, new ConfigDescription("Enables debug logging.", null, new ConfigurationManagerAttributes { Order = 1000 }));
            DisableCulling = cfg.Bind(formatted, "Disable Culling", true,
                new ConfigDescription(
                    "Disables culling while piloting drone. Disabling this will increase FPS but cause objects to render incorrectly while flying at high altitudes.", null,
                    new ConfigurationManagerAttributes { Order = 990 }
                )
            );
            EnableTankPermaDeath = cfg.Bind(formatted, "Enable BTR Permadeath", true,
                new ConfigDescription(
                    "Enables the permanent death of the BTR and it's driver.", null,
                    new ConfigurationManagerAttributes { Order = 980 }
                )
            );
        }
    }
}
#endif
