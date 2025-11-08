using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPVDroneMod.Components
{
    public class DroneHudController : MonoBehaviour
    {
        public Image SignalImage;
        private UIElementStateController _signalStateController;

        public TextMeshProUGUI BatteryText;
        public Image BatteryImage;
        private UIElementStateController _batteryStateController;

        public TextMeshProUGUI ArmedText;
        public TextMeshProUGUI SpeedText;
        public TextMeshProUGUI AltitudeText;

        private void Start()
        {
            _signalStateController = SignalImage.GetComponent<UIElementStateController>();
            _batteryStateController = BatteryImage.GetComponent<UIElementStateController>();

            SetArmedTextVisible(false);
            
            DontDestroyOnLoad(gameObject);
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

        private IEnumerator ToggleArmedText(float duration)
        {
            SetArmedTextVisible(true);
            yield return new WaitForSeconds(duration);
            SetArmedTextVisible(false);
        }
    }
}
