using UnityEngine;
using UnityEngine.UI;

namespace _Scripts._UI {
    public class ManaBar : MonoBehaviour {
        private Slider slider;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        public void SetMaxMana(float maxMana)
        {
            slider.maxValue = maxMana;
            slider.value = maxMana;

        }

        public void SetCurrentMana(float currentMana)
        {
            slider.value = currentMana;
        }
    }
}