using UnityEngine;

namespace _Scripts {
    public class CharacterStats : MonoBehaviour {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        public int staminaLevel = 10;
        public float maxStamina;
        public float currentStamina;

        public int manaLevel = 10;
        public float maxMana;
        public float currentMana;

        public int soulCount = 0; 
        
        public bool isDead;

        public virtual void TakeDamage(int damage,string damageAnimation="[Combat Action] Damage_01")
        {
        }
    }
}