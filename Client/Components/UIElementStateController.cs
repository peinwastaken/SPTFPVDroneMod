using UnityEngine;
using UnityEngine.UI;

namespace FPVDroneMod.Components
{
    public class UIElementStateController : MonoBehaviour
    {
        public Sprite[] states;
        public int currentState = 0;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();

            SetState(states.Length - 1);
        }

        public void SetState01(float normalizedValue)
        {
            normalizedValue = Mathf.Clamp01(normalizedValue);
            int state = Mathf.RoundToInt(normalizedValue * (states.Length - 1));
            SetState(state);
        }

        public void SetState(int state)
        {
            currentState = Mathf.Clamp(state, 0, states.Length - 1);
            _image.sprite = states[currentState];
        }
    }
}
