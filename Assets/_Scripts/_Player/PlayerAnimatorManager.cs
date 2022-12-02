using UnityEngine;

namespace _Scripts._Player {
    public class PlayerAnimatorManager : AnimatorManager {

        private PlayerLocomotion playerLocomotion;
        private PlayerManager playerManager;
        private InputHandler inputHandler;
        private PlayerStats playerStats;

        private int vertical;
        private int horizontal;

        public void Initialize()
        {
            animator = GetComponent<Animator>();

            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            playerManager = GetComponentInParent<PlayerManager>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerStats = GetComponentInParent<PlayerStats>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting = false)
        {
            #region Vertical

            float v = 0;
            if ( verticalMovement > 0 && verticalMovement < 0.55f )
            {
                v = 0.5f;
            }
            else if ( verticalMovement > 0.55f )
            {
                v = 1f;
            }
            else if ( verticalMovement < 0 && verticalMovement > -0.55f )
            {
                v = 0.5f;
            }
            else if ( verticalMovement < -0.55f )
            {
                v = -1f;
            }
            else
            {
                v = 0;
            }

            #endregion

            #region Horizontal

            float h = 0;
            if ( horizontalMovement > 0 && horizontalMovement < 0.55f )
            {
                h = 0.5f;
            }
            else if ( horizontalMovement > 0.55f )
            {
                h = 1f;
            }
            else if ( horizontalMovement < 0 && horizontalMovement > -0.55f )
            {
                h = 0.5f;
            }
            else if ( horizontalMovement < -0.55f )
            {
                h = -1f;
            }
            else
            {
                h = 0;
            }

            #endregion

            if ( isSprinting )
            {
                v = 2;
                h = horizontalMovement;
            }

            animator.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            animator.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }


        #region Animator Events

        public void EnableRotation()
        {
            animator.SetBool("CanRotate", true);
        }

        public void StopRotation()
        {
            animator.SetBool("CanRotate", false);
        }

        public void EnableCombo()
        {
            animator.SetBool("CanDoCombo", true);
        }

        public void DisableCombo()
        {
            animator.SetBool("CanDoCombo", false);
        }

        public void EnableIsInvulnerable()
        {
            animator.SetBool("IsInvulnerable", true);
        }

        public void DisableIsInvulnerable()
        {
            animator.SetBool("IsInvulnerable", false);
        }

        public void EnableIsParrying()
        {
            playerManager.isParrying = true;
        }

        public void DisableIsParrying()
        {
            playerManager.isParrying = false;
        }

        public void EnableCanBeRiposted()
        {
            playerManager.canBeRiposted = true;
        }

        public void DisableCanBeRiposted()
        {
            playerManager.canBeRiposted = false;
        }

        #endregion

        public override void TakeCriticalDamageAnimationEvent()
        {
            base.TakeCriticalDamageAnimationEvent();

            playerStats.TakeCriticalDamage(playerManager.pendingCriticalDamage);
            playerManager.pendingCriticalDamage = 0;
        }

        private void OnAnimatorMove()
        {
            if ( playerManager.isInteracting == false )
                return;

            float delta = Time.deltaTime;
            Debug.LogWarning("Use Root Motion" + animator.deltaPosition);
            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.rigidbody.velocity = velocity;
        }
    }
}