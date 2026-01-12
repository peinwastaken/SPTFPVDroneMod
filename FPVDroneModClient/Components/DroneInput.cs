#if !UNITY_EDITOR
using DXNET.XInput;
using FPVDroneModClient.Config;
using FPVDroneModClient.Helpers;
#endif
using FPVDroneModClient.Interface;
using UnityEngine;

namespace FPVDroneModClient.Components
{
    // TODO: needs a rewrite
    public class DroneInput : MonoBehaviour
    {
        public bool ControllerConnected;
        public IPilotable Pilotable;

        public float LeftStickX;
        public float LeftStickY;
        public float RightStickX;
        public float RightStickY;
        public float LeftTrigger;
        public float RightTrigger;

        public bool ButtonA;
        public bool ButtonX;
        public bool ButtonB;
        public bool ButtonY;

        public bool ButtonRB;
        public bool ButtonLB;

        public float ThrottleInput;
        public float PitchInput;
        public float YawInput;
        public float RollInput;
        public float AltitudeInput;
        public float CameraPitchInput;
        public float CameraZoomInput;

        private bool _prevA;
        private bool _prevB;
        private bool _prevX;
        private bool _prevY;
        private bool _prevRb;
        private bool _prevLb;

        #if !UNITY_EDITOR
        public Controller Controller;
        public Gamepad GamepadState;

        private void Start()
        {
            Pilotable = GetComponent<BaseDroneController>();

            if (ControllerConnected)
            {
                DebugLogger.LogWarning("Controller detected!");
            }
        }

        private void Update()
        {
            GetControllerInput();
            ApplyInput();
        }

        private void OnGUI()
        {
            if (!GeneralConfig.EnableDebug.Value) return;

            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                normal = { textColor = Color.white }
            };

            GUILayout.BeginArea(new Rect(20, 20, 300, 500));
            GUILayout.Label($"Left Stick X: {LeftStickX:F2}", style);
            GUILayout.Label($"Left Stick Y: {LeftStickY:F2}", style);
            GUILayout.Label($"Right Stick X: {RightStickX:F2}", style);
            GUILayout.Label($"Right Stick Y: {RightStickY:F2}", style);
            GUILayout.Label($"Left Trigger: {LeftTrigger:F2}", style);
            GUILayout.Label($"Right Trigger: {RightTrigger:F2}", style);
            GUILayout.Space(10);
            GUILayout.Label($"Button A: {ButtonA}", style);
            GUILayout.Label($"Button B: {ButtonB}", style);
            GUILayout.Label($"Button X: {ButtonX}", style);
            GUILayout.Label($"Button Y: {ButtonY}", style);
            GUILayout.Space(10);
            GUILayout.Label($"Throttle: {ThrottleInput:F2}", style);
            GUILayout.Label($"Pitch: {PitchInput:F2}", style);
            GUILayout.Label($"Yaw: {YawInput:F2}", style);
            GUILayout.Label($"Roll: {RollInput:F2}", style);
            GUILayout.Label($"Altitude: {AltitudeInput:F2}", style);
            GUILayout.EndArea();
        }

        private float NormalizeInput(short value, short deadzone)
        {
            if (value > deadzone)
            {
                return (value - deadzone) / (32767f - deadzone);
            }

            if (value < -deadzone)
            {
                return (value + deadzone) / (32768f - deadzone);
            }

            return 0f;
        }

        private void GetController()
        {
            if (Controller == null)
            {
                Controller = new Controller(UserIndex.One);
            }

            if (Controller != null)
            {
                ControllerConnected = Controller.IsConnected;
            }
            else
            {
                ControllerConnected = false;
            }
        }

        private void GetControllerInput()
        {
            GetController();

            if (ControllerConnected)
            {
                bool ok = Controller.GetState(out State state);

                if (ok)
                {
                    GamepadState = state.Gamepad;

                    LeftStickX = NormalizeInput(GamepadState.LeftThumbX, 7849);
                    LeftStickY = NormalizeInput(GamepadState.LeftThumbY, 7849);
                    RightStickX = NormalizeInput(GamepadState.RightThumbX, 8689);
                    RightStickY = NormalizeInput(GamepadState.RightThumbY, 8689);

                    LeftTrigger = Mathf.Clamp01(GamepadState.LeftTrigger / 255f);
                    RightTrigger = Mathf.Clamp01(GamepadState.RightTrigger / 255f);

                    ButtonA = (GamepadState.Buttons & GamepadButtonFlags.A) != 0;
                    ButtonX = (GamepadState.Buttons & GamepadButtonFlags.X) != 0;
                    ButtonB = (GamepadState.Buttons & GamepadButtonFlags.B) != 0;
                    ButtonY = (GamepadState.Buttons & GamepadButtonFlags.Y) != 0;
                    ButtonRB = (GamepadState.Buttons & GamepadButtonFlags.RightShoulder) != 0;
                    ButtonLB = (GamepadState.Buttons & GamepadButtonFlags.LeftShoulder) != 0;
                }
            }
        }

        private void ApplyInput()
        {
            if (Pilotable is FPVDroneController)
            {
                ApplyFpvInput();
            }
            else if (Pilotable is ReconDroneController)
            {
                ApplyReconInput();
            }
        }

        private void ApplyFpvInput()
        {
            if (ControllerConnected)
            {
                ThrottleInput = LeftStickY;
                AltitudeInput = LeftStickY;
                PitchInput = RightStickY;
                YawInput = LeftStickX;
                RollInput = -RightStickX;

                if (ButtonY && !_prevY && Pilotable is IArmable armable)
                {
                    armable.ToggleArmed();
                }

                if (ButtonB && !_prevB)
                {
                    DroneHelper.ControlDrone(false);
                }

                _prevA = ButtonA;
                _prevB = ButtonB;
                _prevX = ButtonX;
                _prevY = ButtonY;
                _prevRb = ButtonRB;
                _prevLb = ButtonLB;
            }
            else
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                ThrottleInput = Input.GetKey(FPVBindsConfig.Thrust.Value) ? 1f : 0f;
                RollInput = (Input.GetKey(FPVBindsConfig.RollClockwise.Value) ? -1f : 0f) + (Input.GetKey(FPVBindsConfig.RollCounterClockwise.Value) ? 1f : 0f);
                PitchInput = (Input.GetKey(FPVBindsConfig.PitchDown.Value) ? 1f : 0f) + (Input.GetKey(FPVBindsConfig.PitchUp.Value) ? -1f : 0f);
                YawInput = (Input.GetKey(FPVBindsConfig.YawRight.Value) ? 1f : 0f) + (Input.GetKey(FPVBindsConfig.YawLeft.Value) ? -1f : 0f);
                
                if (FPVBindsConfig.MouseEnabled.Value)
                {
                    RollInput += mouseX * FPVBindsConfig.MouseSensitivityX.Value;
                    PitchInput += mouseY * FPVBindsConfig.MouseSensitivityY.Value;
                }

                if (Input.GetKeyDown(FPVBindsConfig.ExitDrone.Value))
                {
                    DroneHelper.ControlDrone(false);
                }

                if (Input.GetKeyDown(FPVBindsConfig.ToggleArmed.Value) && Pilotable is IArmable armable)
                {
                    armable.ToggleArmed();
                }
            }

            PitchInput = Mathf.Clamp(PitchInput, -1f, 1f);
            YawInput = Mathf.Clamp(YawInput, -1f, 1f);
            RollInput = Mathf.Clamp(RollInput, -1f, 1f);
            ThrottleInput = Mathf.Clamp(ThrottleInput, -1f, 1f);
        }

        private void ApplyReconInput()
        {
            if (ControllerConnected)
            {
                AltitudeInput = LeftStickY;
                PitchInput = RightStickY;
                YawInput = LeftStickX;
                RollInput = RightStickX;
                CameraPitchInput += LeftTrigger;
                CameraPitchInput -= RightTrigger;
                CameraZoomInput = (ButtonRB ? 1f : 0f) + (ButtonLB ? -1f : 0f);
                
                if (ButtonY && !_prevY && Pilotable is IArmable armable)
                {
                    armable.ToggleArmed();
                }

                if (ButtonB && !_prevB)
                {
                    DroneHelper.ControlDrone(false);
                }

                _prevA = ButtonA;
                _prevB = ButtonB;
                _prevX = ButtonX;
                _prevY = ButtonY;
                _prevRb = ButtonRB;
                _prevLb = ButtonLB;
            }
            else
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
                
                AltitudeInput = (Input.GetKey(ReconBindsConfig.ThrustUp.Value) ? 1f : 0f) + (Input.GetKey(ReconBindsConfig.ThrustDown.Value) ? -1f : 0f);
                PitchInput = (Input.GetKey(ReconBindsConfig.PitchDown.Value) ? 1f : 0f) + (Input.GetKey(ReconBindsConfig.PitchUp.Value) ? -1f : 0f);
                RollInput = (Input.GetKey(ReconBindsConfig.RollClockwise.Value) ? 1f : 0f) + (Input.GetKey(ReconBindsConfig.RollCounterClockwise.Value) ? -1f : 0f);
                YawInput = (Input.GetKey(ReconBindsConfig.YawRight.Value) ? 1f : 0f) + (Input.GetKey(ReconBindsConfig.YawLeft.Value) ? -1f : 0f);
                CameraPitchInput = (Input.GetKey(ReconBindsConfig.CameraPitchUp.Value) ? 1f : 0f) + (Input.GetKey(ReconBindsConfig.CameraPitchDown.Value) ? -1f : 0f);
                CameraZoomInput = (Input.GetKey(ReconBindsConfig.CameraZoomIn.Value) ? 1f : 0f) + (Input.GetKey(ReconBindsConfig.CameraZoomOut.Value) ? -1f : 0f);
                
                if (ReconBindsConfig.MouseEnabled.Value)
                {
                    YawInput += mouseX * ReconBindsConfig.MouseSensitivityX.Value;
                    CameraPitchInput += mouseY * ReconBindsConfig.MouseSensitivityY.Value;
                    CameraZoomInput += mouseScroll * ReconBindsConfig.MouseScrollSensitivity.Value;
                }

                if (Input.GetKeyDown(ReconBindsConfig.ExitDrone.Value))
                {
                    DroneHelper.ControlDrone(false);
                }
            }

            PitchInput = Mathf.Clamp(PitchInput, -1f, 1f);
            YawInput = Mathf.Clamp(YawInput, -1f, 1f);
            RollInput = Mathf.Clamp(RollInput, -1f, 1f);
            ThrottleInput = Mathf.Clamp(ThrottleInput, -1f, 1f);
        }
        #endif
    }
}
