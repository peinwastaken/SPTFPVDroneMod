using BepInEx.Configuration;
using FPVDroneMod.Globals;

namespace FPVDroneMod.Config
{
    public class ExplosionConfig
    {
        /*
         public class ExplosionData
    {
        public Vector3 Position = Vector3.zero;
        public float MaxDistance = 5f;
        public float Damage = 200f;
        public string EffectName = "Grenade_new";
        public float FractureDelta = 0.6f;
        public float HeavyBleedDelta = 0.4f;
        public float LightBleedDelta = 0.7f;
        public float StaminaBurnRate = 0.7f;
        public float InstantKillDistance = -1f;
        public Vector3 EffectDirection = Vector3.up;
        public IPlayerOwner PlayerOwner = null;
        public Item Weapon = null;
    }
         */

        public static ConfigEntry<float> ExplosionMaxDistance;
        public static ConfigEntry<float> ExplosionDamage;
        public static ConfigEntry<float> ExplosionFractureDelta;
        public static ConfigEntry<float> ExplosionHeavyBleedDelta;
        public static ConfigEntry<float> ExplosionLightBleedDelta;
        public static ConfigEntry<float> ExplosionStaminaBurnRate;

        public static void Bind(int order, string category, ConfigFile cfg)
        {
            string formatted = Category.Format(order, category);

            ExplosionMaxDistance = cfg.Bind(formatted, "Explosion Max Distance", 5f, new ConfigDescription(
                "Maximum explosion range",
                null,
                new ConfigurationManagerAttributes { Order = 1000 }));

            ExplosionDamage = cfg.Bind(formatted, "Explosion Damage", 200f, new ConfigDescription(
                "Base explosion damage amount",
                null,
                new ConfigurationManagerAttributes { Order = 990 }));

            ExplosionFractureDelta = cfg.Bind(formatted, "Explosion Fracture Delta", 0.6f, new ConfigDescription(
                "Fracture chance modifier",
                null,
                new ConfigurationManagerAttributes { Order = 980 }));

            ExplosionHeavyBleedDelta = cfg.Bind(formatted, "Explosion Heavy Bleed Delta", 0.4f, new ConfigDescription(
                "Heavy bleed chance modifier",
                null,
                new ConfigurationManagerAttributes { Order = 970 }));

            ExplosionLightBleedDelta = cfg.Bind(formatted, "Explosion Light Bleed Delta", 0.7f, new ConfigDescription(
                "Light bleed chance modifier",
                null,
                new ConfigurationManagerAttributes { Order = 960 }));

            ExplosionStaminaBurnRate = cfg.Bind(formatted, "Explosion Stamina Burn Rate", 0.7f, new ConfigDescription(
                "Stamina burn modifier. And stuff.",
                null,
                new ConfigurationManagerAttributes { Order = 950 }));
        }
    }
}
