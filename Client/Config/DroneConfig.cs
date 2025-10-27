using BepInEx.Configuration;
using FPVDroneMod.Globals;

namespace FPVDroneMod.Config
{
    public static class DroneConfig
    {
        // drone physics settings
        public static ConfigEntry<float> DroneMass;
        public static ConfigEntry<float> DroneThrustForce;
        public static ConfigEntry<float> DroneMaxVelocity;

        // drone rotation settings
        public static ConfigEntry<float> DronePitchSpeed;
        public static ConfigEntry<float> DroneYawSpeed;
        public static ConfigEntry<float> DroneRollSpeed;

        // drone propeller settings
        public static ConfigEntry<float> DronePropellerAccelerationSpeed;
        public static ConfigEntry<float> DroneMinPropellerSpeed;
        public static ConfigEntry<float> DroneMaxPropellerSpeed;

        // drone battery settings
        public static ConfigEntry<float> DroneMaxBattery;
        public static ConfigEntry<float> DroneBatteryDecayIdle;
        public static ConfigEntry<float> DroneBatteryDecayAccel;

        public static void Bind(int order, string category, ConfigFile cfg)
        {
            string formatted = Category.Format(order, category);

            // drone physics settings
            DroneMass = cfg.Bind(formatted, "Drone Mass", 1f, new ConfigDescription(
                "changes drone mass",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes() { Order = 1000 }));

            DroneThrustForce = cfg.Bind(formatted, "Drone Thrust Force", 50f, new ConfigDescription(
                "changes the thrust force of the drone",
                null,
                new ConfigurationManagerAttributes() { Order = 990 }));

            DroneMaxVelocity = cfg.Bind(formatted, "Drone Max Velocity", 25f, new ConfigDescription(
                "maximum speed the drone can reach",
                null,
                new ConfigurationManagerAttributes() { Order = 980 }));

            // drone rotation settings
            DronePitchSpeed = cfg.Bind(formatted, "Drone Pitch Speed", 100f, new ConfigDescription(
                "pitch rotation speed of the drone",
                null,
                new ConfigurationManagerAttributes() { Order = 970 }));

            DroneYawSpeed = cfg.Bind(formatted, "Drone Yaw Speed", 100f, new ConfigDescription(
                "yaw rotation speed of the drone",
                null,
                new ConfigurationManagerAttributes() { Order = 960 }));

            DroneRollSpeed = cfg.Bind(formatted, "Drone Roll Speed", 100f, new ConfigDescription(
                "roll rotation speed of the drone",
                null,
                new ConfigurationManagerAttributes() { Order = 950 }));

            // drone propeller settings
            DronePropellerAccelerationSpeed = cfg.Bind(formatted, "Drone Propeller Acceleration Speed", 4f, new ConfigDescription(
                "acceleration speed of the propellers",
                null,
                new ConfigurationManagerAttributes() { Order = 940 }));

            DroneMinPropellerSpeed = cfg.Bind(formatted, "Drone Min Propeller Speed", 0f, new ConfigDescription(
                "minimum propeller speed",
                null,
                new ConfigurationManagerAttributes() { Order = 930 }));

            DroneMaxPropellerSpeed = cfg.Bind(formatted, "Drone Max Propeller Speed", 10000f, new ConfigDescription(
                "maximum propeller speed",
                null,
                new ConfigurationManagerAttributes() { Order = 920 }));

            // drone battery settings
            DroneMaxBattery = cfg.Bind(formatted, "Drone Max Battery", 150f, new ConfigDescription(
                "maximum battery capacity",
                null,
                new ConfigurationManagerAttributes() { Order = 910 }));

            DroneBatteryDecayIdle = cfg.Bind(formatted, "Drone Battery Decay (Idle)", 0.001f, new ConfigDescription(
                "battery decay rate when idle",
                null,
                new ConfigurationManagerAttributes() { Order = 900 }));

            DroneBatteryDecayAccel = cfg.Bind(formatted, "Drone Battery Decay (Accel)", 0.01f, new ConfigDescription(
                "battery decay rate when accelerating",
                null,
                new ConfigurationManagerAttributes() { Order = 890 }));
        }
    }
}
