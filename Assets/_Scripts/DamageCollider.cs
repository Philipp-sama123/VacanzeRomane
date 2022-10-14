using _Scripts._AI;
using _Scripts._Player;
using UnityEngine;

namespace _Scripts {
    [RequireComponent(typeof( Collider ))]
    public class DamageCollider : MonoBehaviour {
        public CharacterManager characterManager; // gets loaded on LoadWeaponsDamageCollider // TODO: challenge this
        private Collider damageCollider;
        
        public bool enabledColliderOnStartup = false; 
        public int currentWeaponDamage = 25;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = enabledColliderOnStartup;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            Debug.Log("[Info] Attack registered on:  " + collision.name + " from " + gameObject.name);

            if ( collision.CompareTag($"Player") )
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();
                CharacterManager enemyCharacterManager = collision.GetComponent<CharacterManager>();

                BlockingCollider shield = collision.GetComponentInChildren<BlockingCollider>();

                if ( enemyCharacterManager != null )
                {
                    if ( enemyCharacterManager.isParrying )
                    {
                        characterManager.GetComponentInChildren<AnimatorManager>()
                            .PlayTargetAnimation("[Combat Action] Parried", true);
                        return;
                    }
                    else if ( shield != null && enemyCharacterManager.isBlocking )
                    {
                        float physicalDamageAfterBlock = currentWeaponDamage - (currentWeaponDamage * shield.blockingPhysicalDamageAbsorption / 100);

                        if ( playerStats != null )
                        {
                            playerStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "[Combat Action] Blocking Hit");

                        }
                    }
                    else
                    {
                        if ( playerStats != null )
                        {
                            // Todo: interface in Stats
                            playerStats.TakeDamage(currentWeaponDamage);
                        }
                    }
                }
            }

            if ( collision.CompareTag($"Enemy") )
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
                CharacterManager enemyCharacterManager = collision.GetComponent<CharacterManager>();

                BlockingCollider shield = collision.GetComponentInChildren<BlockingCollider>();

                if ( enemyCharacterManager != null )
                {
                    if ( enemyCharacterManager.isParrying )
                    {
                        characterManager.GetComponentInChildren<AnimatorManager>()
                            .PlayTargetAnimation("[Combat Action] Parried", true);
                    }
                    else if ( shield != null && enemyCharacterManager.isBlocking )
                    {
                        float physicalDamageAfterBlock = currentWeaponDamage - (currentWeaponDamage * shield.blockingPhysicalDamageAbsorption / 100);

                        if ( enemyStats != null )
                        {
                            enemyStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "[Combat Action] Blocking Hit");

                        }
                    }
                    else
                    {
                        if ( enemyStats != null )
                        {
                            // Todo: interface in Stats
                            enemyStats.TakeDamage(currentWeaponDamage);
                        }
                    }
                }
            }
        }
    }
}