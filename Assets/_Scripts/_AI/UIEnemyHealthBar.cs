using UnityEngine;
using UnityEngine.UI;

namespace _Scripts._AI {
    public class UIEnemyHealthBar : MonoBehaviour {
        private Slider slider;
        public float timeUntilBarIsHidden = 0f;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        public void SetHealth(int health)
        {
            slider.value = health;
            timeUntilBarIsHidden = 5f;
        }

        public void SetMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        private void Update()
        {
            timeUntilBarIsHidden = timeUntilBarIsHidden - Time.deltaTime;
            if ( slider != null )
            {
                if ( timeUntilBarIsHidden <= 0 )
                {
                    timeUntilBarIsHidden = 0;
                    slider.gameObject.SetActive(false);
                }
                else
                {
                    if ( !slider.gameObject.activeInHierarchy )
                    {
                        slider.gameObject.SetActive(true);
                    }
                }
                if ( slider.value <= 0 )
                {
                    Destroy(slider.gameObject);
                }
            }

        }
    }
}