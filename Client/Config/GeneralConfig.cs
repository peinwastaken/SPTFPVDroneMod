#if !UNITY_EDITOR
using BepInEx.Configuration;
using FPVDroneMod.Globals;

namespace FPVDroneMod.Config
{
    public static class GeneralConfig
    {
        public static ConfigEntry<bool> EnableDebug { get; set; }
        
        public static void Bind(int order, string category, ConfigFile cfg)
        {
            string formatted = Category.Format(order, category);

            EnableDebug = cfg.Bind(formatted, "Enable Debug Logging", false, "Enables debug logging.");
        }
    }
}
#endif
