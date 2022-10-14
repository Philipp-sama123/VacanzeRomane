namespace _Scripts._AI {
    public class EnemyStats : CharacterStats {
        private EnemyAnimatorManager enemyAnimatorManager;
        public UIEnemyHealthBar enemyHealthBar;

        public int soulsAwardedOnDeath = 10;

        private void Awake()
        {
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            enemyHealthBar.SetMaxHealth(maxHealth);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public override void TakeDamage(int damage, string damageAnimation = "[Combat Action] Damage_01")
        {
            if ( isDead ) return;

            currentHealth = currentHealth - damage;

            enemyAnimatorManager.PlayTargetAnimation(damageAnimation, true);
            enemyHealthBar.SetHealth(currentHealth);

            if ( currentHealth <= 0 )
            {
                HandleDeath();
            }
        }

        public void TakeCriticalDamage(int criticalDamage)
        {
            currentHealth = currentHealth - criticalDamage;
            enemyHealthBar.SetHealth(currentHealth);

            if ( currentHealth <= 0 )
            {
                currentHealth = 0;
                isDead = true;
            }
        }

        private void HandleDeath()
        {
            currentHealth = 0;
            enemyAnimatorManager.PlayTargetAnimation("[Death] Rolling Death", true);
            isDead = true;
        }
    }
}