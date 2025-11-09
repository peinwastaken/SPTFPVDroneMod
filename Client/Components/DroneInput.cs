using DXNET;
using FPVDroneMod.Config;
using FPVDroneMod.Helpers;
using DXNET.XInput;
using UnityEngine;

namespace FPVDroneMod.Components
{
    public class DroneInput : MonoBehaviour
    {
        public Controller Controller;
        public Gamepad GamepadState;
        public bool ControllerConnected = false;
        public DroneController DroneController;

        public float LeftStickX = 0f;
        public float LeftStickY = 0f;
        public float RightStickX = 0f;
        public float RightStickY = 0f;
        public float LeftTrigger = 0f;
        public float RightTrigger = 0f;

        public bool ButtonA = false;
        public bool ButtonX = false;
        public bool ButtonB = false;
        public bool ButtonY = false;
        
        private bool _prevA = false;
        private bool _prevB = false;
        private bool _prevX = false;
        private bool _prevY = false;

        public float ThrottleInput = 0f;
        public float PitchInput = 0f;
        public float YawInput = 0f;
        public float RollInput = 0f;
        
        private void Start()
        {
            DroneController = GetComponent<DroneController>();

            if (ControllerConnected)
            {
                DebugLogger.LogWarning("Controller detected!");
            }
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
                }
            }
        }

        private void ApplyInput()
        {
            if (ControllerConnected)
            {
                ThrottleInput = LeftStickY;
                PitchInput = RightStickY;
                YawInput = LeftStickX;
                RollInput = -RightStickX;

                if (ButtonY && !_prevY)
                {
                    DroneController.ToggleArmed();
                }

                if (ButtonB && !_prevB)
                {
                    DroneHelper.ControlDrone(false);
                }

                _prevA = ButtonA;
                _prevB = ButtonB;
                _prevX = ButtonX;
                _prevY = ButtonY;
            }
            else
            {
                ThrottleInput = Input.GetKey(BindsConfig.Thrust.Value) ? 1f : 0f;
                RollInput = (Input.GetKey(BindsConfig.RollClockwise.Value) ? -1f : 0f) + (Input.GetKey(BindsConfig.RollCounterClockwise.Value) ? 1f : 0f);
                PitchInput = (Input.GetKey(BindsConfig.PitchDown.Value) ? 1f : 0f) + (Input.GetKey(BindsConfig.PitchUp.Value) ? -1f : 0f);
                YawInput = (Input.GetKey(BindsConfig.YawRight.Value) ? 1f : 0f) + (Input.GetKey(BindsConfig.YawLeft.Value) ? -1f : 0f);
                
                if (Input.GetKeyDown(BindsConfig.ExitDrone.Value))
                {
                    DroneHelper.ControlDrone(false);
                }
            
                if (Input.GetKeyDown(BindsConfig.ToggleArmed.Value))
                {
                    DroneController.ToggleArmed();
                }
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

            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
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
            GUILayout.EndArea();
        }
    }
}
