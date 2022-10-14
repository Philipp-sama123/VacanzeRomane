using System;
using _Scripts._UI;
using UnityEngine;

namespace _Scripts._Player {
    public class PlayerStats : CharacterStats {
        public StaminaBar staminaBar;
        public HealthBar healthBar;
        public ManaBar manaBar;

        PlayerManager playerManager;
        PlayerAnimatorManager playerAnimatorManager;

        public float staminaRegenerationAmount = 2.5f;
        public float staminaRegenerationTimer = 0;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();

            staminaBar = FindObjectOfType<StaminaBar>();
            healthBar = FindObjectOfType<HealthBar>();
            manaBar = FindObjectOfType<ManaBar>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(maxStamina);
            staminaBar.SetCurrentStamina(currentStamina);

            maxMana = SetMaxManaFromManaLevel();
            currentMana = maxMana;
            manaBar.SetMaxMana(maxMana);
            manaBar.SetCurrentMana(currentMana);

        }

        // ToDo: think of a nice System and maybe refactor them to a "StatBar"
        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private float SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        private float SetMaxManaFromManaLevel()
        {
            maxMana = manaLevel * 10;
            return maxMana;
        }

        public override void TakeDamage(int damage,string damageAnimation="[Combat Action] Damage_01" )
        {
            if ( playerManager.isInvulnerable ) return;
            if ( isDead ) return;

            currentHealth = currentHealth - damage;

            healthBar.SetCurrentHealth(currentHealth);

            playerAnimatorManager.PlayTargetAnimation(damageAnimation, true);

            if ( currentHealth <= 0 )
            {
                currentHealth = 0;
                playerAnimatorManager.PlayTargetAnimation("[Death] Honor Death", true);
                isDead = true;
            }
        }

        public void TakeCriticalDamage(int criticalDamage)
        {
            if ( playerManager.isInvulnerable ) return;

            currentHealth = currentHealth - criticalDamage;
            healthBar.SetCurrentHealth(currentHealth);

            if ( currentHealth <= 0 )
            {
                currentHealth = 0;
                isDead = true;
            }
        }

        public void TakeStaminaDamage(float damage)
        {
            currentStamina = currentStamina - damage;
            staminaBar.SetCurrentStamina(currentStamina);
            // TODO 
            // Stamina Drain Animation
        }

        public void RegenerateStamina()
        {
            if ( playerManager.isInteracting )
            {
                staminaRegenerationTimer = 0;
            }
            else
            {
                staminaRegenerationTimer += Time.deltaTime;

                if ( currentStamina < maxStamina && staminaRegenerationTimer > 1f )
                {
                    currentStamina += staminaRegenerationAmount * Time.deltaTime;
                    staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                }
            }
        }

        public void HealPlayer(int healAmount)
        {
            currentHealth = currentHealth + healAmount;

            if ( currentHealth > maxHealth )
            {
                currentHealth = maxHealth;
            }
            healthBar.SetCurrentHealth(currentHealth);
        }

        public void ReduceMana(int manaCost)
        {
            currentMana = currentMana - manaCost;
            if ( currentMana < 0 )
            {
                currentMana = 0;
            }
            manaBar.SetCurrentMana(currentMana);
        }

        public void AddSouls(int souls)
        {
            soulCount = soulCount + souls;
        }
    }
}