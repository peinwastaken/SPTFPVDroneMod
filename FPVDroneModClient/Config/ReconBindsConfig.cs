#if !UNITY_EDITOR
using BepInEx.Configuration;
using FPVDroneModClient.Globals;
using FPVDroneModClient.Helpers;
using UnityEngine;

namespace FPVDroneModClient.Config
{
    public static class ReconBindsConfig
    {
        public static ConfigEntry<KeyCode> ThrustUp;
        public static ConfigEntry<KeyCode> ThrustDown;
        public static ConfigEntry<KeyCode> YawLeft;
        public static ConfigEntry<KeyCode> YawRight;
        public static ConfigEntry<KeyCode> PitchUp;
        public static ConfigEntry<KeyCode> PitchDown;
        public static ConfigEntry<KeyCode> RollClockwise;
        public static ConfigEntry<KeyCode> RollCounterClockwise;
        public static ConfigEntry<KeyCode> ExitDrone;
        public static ConfigEntry<KeyCode> CameraZoomIn;
        public static ConfigEntry<KeyCode> CameraZoomOut;
        public static ConfigEntry<KeyCode> CameraPitchUp;
        public static ConfigEntry<KeyCode> CameraPitchDown;
        public static ConfigEntry<bool> MouseEnabled;
        public static ConfigEntry<float> MouseSensitivityX;
        public static ConfigEntry<float> MouseSensitivityY;
        public static ConfigEntry<float> MouseScrollSensitivity;

        public static void Bind(int order, string category, ConfigFile cfg)
        {
            string formatted = Category.Format(order, category);

            // FPV DRONE CONTROLS
            ThrustUp = cfg.Bind(formatted, "Thrust Up", KeyCode.Space, new ConfigDescription(
                "Apply thrust",
                null,
                new ConfigurationManagerAttributes { Order = 1000 }));
            
            ThrustDown = cfg.Bind(formatted, "Thrust Down", KeyCode.LeftShift, new ConfigDescription(
                "Apply thrust",
                null,
                new ConfigurationManagerAttributes { Order = 995 }));

            YawLeft = cfg.Bind(formatted, "Yaw Left", KeyCode.LeftArrow, new ConfigDescription(
                "Drone yaw left",
                null,
                new ConfigurationManagerAttributes { Order = 990 }));

            YawRight = cfg.Bind(formatted, "Yaw Right", KeyCode.RightArrow, new ConfigDescription(
                "Drone yaw right",
                null,
                new ConfigurationManagerAttributes { Order = 980 }));

            PitchUp = cfg.Bind(formatted, "Pitch Up", KeyCode.S, new ConfigDescription(
                "Drone pitch up",
                null,
                new ConfigurationManagerAttributes { Order = 970 }));

            PitchDown = cfg.Bind(formatted, "Pitch Down", KeyCode.W, new ConfigDescription(
                "Drone pitch down",
                null,
                new ConfigurationManagerAttributes { Order = 960 }));

            RollClockwise = cfg.Bind(formatted, "Roll Clockwise", KeyCode.D, new ConfigDescription(
                "Roll clockwise",
                null,
                new ConfigurationManagerAttributes { Order = 950 }));

            RollCounterClockwise = cfg.Bind(formatted, "Roll Counterclockwise", KeyCode.A, new ConfigDescription(
                "Roll counter-clockwise",
                null,
                new ConfigurationManagerAttributes { Order = 940 }));

            ExitDrone = cfg.Bind(formatted, "Exit Drone", KeyCode.Backspace, new ConfigDescription(
                "Exit drone view",
                null,
                new ConfigurationManagerAttributes { Order = 920 }));

            CameraZoomIn = cfg.Bind(formatted, "Camera Zoom In", KeyCode.E, new ConfigDescription(
                "Zoom camera in",
                null,
                new ConfigurationManagerAttributes { Order = 870 }));

            CameraZoomOut = cfg.Bind(formatted, "Camera Zoom Out", KeyCode.Q, new ConfigDescription(
                "Zoom camera out",
                null,
                new ConfigurationManagerAttributes { Order = 860 }));
            
            CameraPitchDown = cfg.Bind(formatted, "Camera Pitch Down", KeyCode.DownArrow, new ConfigDescription(
                "Zoom pitch down",
                null,
                new ConfigurationManagerAttributes { Order = 870 }));
            
            CameraPitchUp = cfg.Bind(formatted, "Camera Pitch Up", KeyCode.UpArrow, new ConfigDescription(
                "Zoom pitch up",
                null,
                new ConfigurationManagerAttributes { Order = 870 }));

            // MOUSE CONTROLS
            MouseEnabled = cfg.Bind(formatted, "Enable Mouse Controls", true, new ConfigDescription(
                "Enables mouse controls",
                null,
                new ConfigurationManagerAttributes { Order = 850 }));

            MouseSensitivityX = cfg.Bind(formatted, "Mouse Sensitivity X", 0.05f, new ConfigDescription(
                "Mouse sensitivity X",
                new AcceptableValueRange<float>(-5f, 5f),
                new ConfigurationManagerAttributes { Order = 840 }));

            MouseSensitivityY = cfg.Bind(formatted, "Mouse Sensitivity Y", -0.6f, new ConfigDescription(
                "Mouse sensitivity Y",
                new AcceptableValueRange<float>(-5f, 5f),
                new ConfigurationManagerAttributes { Order = 830 }));
            
            MouseScrollSensitivity = cfg.Bind(formatted, "Mouse Scroll Sensitivity", 0.2f, new ConfigDescription(
                "Mouse scroll sensitivity",
                new AcceptableValueRange<float>(-5f, 5f),
                new ConfigurationManagerAttributes { Order = 830 }));
        }
    }
}
#endif
