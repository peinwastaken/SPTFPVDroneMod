#if !UNITY_EDITOR
using BepInEx.Configuration;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using UnityEngine;

namespace FPVDroneModClient.Config
{
    public static class BindsConfig
    {
        public static ConfigEntry<KeyCode> Thrust;
        public static ConfigEntry<KeyCode> YawLeft;
        public static ConfigEntry<KeyCode> YawRight;
        public static ConfigEntry<KeyCode> PitchUp;
        public static ConfigEntry<KeyCode> PitchDown;
        public static ConfigEntry<KeyCode> RollClockwise;
        public static ConfigEntry<KeyCode> RollCounterClockwise;
        public static ConfigEntry<KeyCode> ToggleArmed;
        public static ConfigEntry<KeyCode> ExitDrone;
        public static ConfigEntry<KeyCode> AltitudeUp;
        public static ConfigEntry<KeyCode> AltitudeDown;
        public static ConfigEntry<KeyCode> CameraZoomIn;
        public static ConfigEntry<KeyCode> CameraZoomOut;
        public static ConfigEntry<bool> MouseEnabled;
        public static ConfigEntry<float> MouseSensitivityX;
        public static ConfigEntry<float> MouseSensitivityY;

        public static void Bind(int order, string category, ConfigFile cfg)
        {
            string formatted = Category.Format(order, category);

            // FPV DRONE CONTROLS
            Thrust = cfg.Bind(formatted, "Thrust", KeyCode.W, new ConfigDescription(
                "Apply thrust",
                null,
                new ConfigurationManagerAttributes { Order = 1000 }));

            YawLeft = cfg.Bind(formatted, "Yaw Left", KeyCode.A, new ConfigDescription(
                "Drone yaw left",
                null,
                new ConfigurationManagerAttributes { Order = 990 }));

            YawRight = cfg.Bind(formatted, "Yaw Right", KeyCode.D, new ConfigDescription(
                "Drone yaw right",
                null,
                new ConfigurationManagerAttributes { Order = 980 }));

            PitchUp = cfg.Bind(formatted, "Pitch Up", KeyCode.DownArrow, new ConfigDescription(
                "Drone pitch up",
                null,
                new ConfigurationManagerAttributes { Order = 970 }));

            PitchDown = cfg.Bind(formatted, "Pitch Down", KeyCode.UpArrow, new ConfigDescription(
                "Drone pitch down",
                null,
                new ConfigurationManagerAttributes { Order = 960 }));

            RollClockwise = cfg.Bind(formatted, "Roll Clockwise", KeyCode.RightArrow, new ConfigDescription(
                "Roll clockwise",
                null,
                new ConfigurationManagerAttributes { Order = 950 }));

            RollCounterClockwise = cfg.Bind(formatted, "Roll Counterclockwise", KeyCode.LeftArrow, new ConfigDescription(
                "Roll counter-clockwise",
                null,
                new ConfigurationManagerAttributes { Order = 940 }));

            ToggleArmed = cfg.Bind(formatted, "Toggle Armed", KeyCode.K, new ConfigDescription(
                "Toggle drone armed state",
                null,
                new ConfigurationManagerAttributes { Order = 930 }));

            ExitDrone = cfg.Bind(formatted, "Exit Drone", KeyCode.Backspace, new ConfigDescription(
                "Exit drone view",
                null,
                new ConfigurationManagerAttributes { Order = 920 }));
            
            // RECON DRONE CONTROLS
            AltitudeUp = cfg.Bind(formatted, "Altitude Up", KeyCode.Space, new ConfigDescription(
                "Increase drone altitude",
                null,
                new ConfigurationManagerAttributes { Order = 910 }));

            AltitudeDown = cfg.Bind(formatted, "Altitude Down", KeyCode.LeftShift, new ConfigDescription(
                "Decrease drone altitude",
                null,
                new ConfigurationManagerAttributes { Order = 900 }));

            CameraZoomIn = cfg.Bind(formatted, "Camera Zoom In", KeyCode.Plus, new ConfigDescription(
                "Zoom camera in",
                null,
                new ConfigurationManagerAttributes { Order = 870 }));

            CameraZoomOut = cfg.Bind(formatted, "Camera Zoom Out", KeyCode.Minus, new ConfigDescription(
                "Zoom camera out",
                null,
                new ConfigurationManagerAttributes { Order = 860 }));

            // MOUSE CONTROLS
            MouseEnabled = cfg.Bind(formatted, "Enable Mouse Controls", true, new ConfigDescription(
                "Enables mouse controls",
                null,
                new ConfigurationManagerAttributes { Order = 850 }));

            MouseSensitivityX = cfg.Bind(formatted, "Mouse Sensitivity X", -2f, new ConfigDescription(
                "Mouse sensitivity X",
                new AcceptableValueRange<float>(-5f, 5f),
                new ConfigurationManagerAttributes { Order = 840 }));

            MouseSensitivityY = cfg.Bind(formatted, "Mouse Sensitivity Y", 2f, new ConfigDescription(
                "Mouse sensitivity Y",
                new AcceptableValueRange<float>(-5f, 5f),
                new ConfigurationManagerAttributes { Order = 830 }));
        }
    }
}
#endif
