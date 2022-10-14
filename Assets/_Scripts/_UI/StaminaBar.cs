using UnityEngine;
using UnityEngine.UI;

namespace _Scripts {
    public class StaminaBar : MonoBehaviour {
        private Slider slider;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        public void SetMaxStamina(float maxStamina)
        {
            slider.maxValue = maxStamina;
            slider.value = maxStamina;

        }

        public void SetCurrentStamina(float currentStamina)
        {
            slider.value = currentStamina;
        }
    }
}