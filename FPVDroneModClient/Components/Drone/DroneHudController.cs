using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPVDroneModClient.Components.Drone
{
    public class DroneHudController : MonoBehaviour
    {
        public Image SignalImage;
        private UIElementStateController _signalStateController;

        public TextMeshProUGUI BatteryText;
        public Image BatteryImage;
        private UIElementStateController _batteryStateController;

        public TextMeshProUGUI ArmedText;
        public TextMeshProUGUI CustomizedText;
        public TextMeshProUGUI SpeedText;
        public TextMeshProUGUI AltitudeText;

        private void Awake()
        {
            _signalStateController = SignalImage.GetComponent<UIElementStateController>();
            _batteryStateController = BatteryImage.GetComponent<UIElementStateController>();

            if (ArmedText)
            {
                SetArmedTextVisible(false);
            }
        }

        public void UpdateSpeed(float speed)
        {
            SpeedText.text = Math.Floor(speed).ToString();
        }

        public void UpdateAltitude(float altitude)
        {
            AltitudeText.text = Math.Floor(altitude).ToString();
        }

        public void UpdateSignalStrength(float strength)
        {
            _signalStateController.SetState01(strength);
        }

        public void UpdateBatteryLevel(float level)
        {
            BatteryText.text = $"{Mathf.RoundToInt(level * 100f)} %";
            _batteryStateController.SetState01(level);
        }

        public void ShowArmedText(float duration)
        {
            StartCoroutine(ToggleArmedText(duration));
        }

        public void SetArmedTextVisible(bool visible)
        {
            ArmedText.gameObject.SetActive(visible);
        }

        public void SetArmedText(string text)
        {
            if (ArmedText)
            {
                ArmedText.text = text;
            }
        }

        public void SetCustomizedText(string text)
        {
            if (CustomizedText)
            {
                CustomizedText.text = text;
            }
        }

        private IEnumerator ToggleArmedText(float duration)
        {
            SetArmedTextVisible(true);
            yield return new WaitForSeconds(duration);
            SetArmedTextVisible(false);
        }
    }
}
