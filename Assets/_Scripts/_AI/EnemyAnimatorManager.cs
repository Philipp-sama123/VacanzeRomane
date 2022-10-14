using _Scripts._Player;
using _Scripts._UI;
using UnityEngine;

namespace _Scripts._AI {
    public class EnemyAnimatorManager : AnimatorManager {
        private EnemyManager enemyManager;
        private EnemyStats enemyStats;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            enemyStats = GetComponentInParent<EnemyStats>();
            enemyManager = GetComponentInParent<EnemyManager>();
        }

        public void AwardSoulsOnDeath()
        {
            // scan for every player and reward them! 
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            SoulCountBar soulCountBar = FindObjectOfType<SoulCountBar>();

            if ( playerStats != null )
            {
                playerStats.AddSouls(enemyStats.soulsAwardedOnDeath);
                if ( soulCountBar != null )
                {
                    soulCountBar.SetSoulCountText(playerStats.soulCount);
                }
            }
        }

        public override void TakeCriticalDamageAnimationEvent()
        {
            base.TakeCriticalDamageAnimationEvent();

            enemyStats.TakeCriticalDamage(enemyManager.pendingCriticalDamage);
            enemyManager.pendingCriticalDamage = 0;
        }

        #region Animator Events

        public void EnableCombo()
        {
            animator.SetBool("CanDoCombo", true);
        }

        public void DisableCombo()
        {
            animator.SetBool("CanDoCombo", false);
        }

        public void EnableRotation()
        {
            animator.SetBool("CanRotate", true);
        }

        public void StopRotation()
        {
            animator.SetBool("CanRotate", false);
        }
        
        public void EnableIsParrying()
        {
            enemyManager.isParrying = true;
        }

        public void DisableIsParrying()
        {
            enemyManager.isParrying = false;
        }

        public void EnableCanBeRiposted()
        {
            enemyManager.canBeRiposted = true; 
        }
        public void DisableCanBeRiposted()
        {
            enemyManager.canBeRiposted = false; 
        }

        #endregion

        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            enemyManager.enemyRigidBody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyManager.enemyRigidBody.velocity = velocity;
        }
    }
}