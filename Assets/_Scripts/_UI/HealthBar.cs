using UnityEngine;
using UnityEngine.UI;

namespace _Scripts {
    public class HealthBar : MonoBehaviour {
        private Slider slider;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        public void SetMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;

        }

        public void SetCurrentHealth(int currentHealth)
        {
            slider.value = currentHealth;
        }
    }
}