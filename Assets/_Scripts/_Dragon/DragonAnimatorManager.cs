using UnityEngine;

namespace _Scripts._Dragon {
    [RequireComponent(typeof( Animator ))]
    public class DragonAnimatorManager : MonoBehaviour {
        public Animator animator { get; private set; }
        private DragonManager dragonManager;
        private DragonLocomotion dragonLocomotion;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            dragonManager = GetComponent<DragonManager>();
            dragonLocomotion = GetComponent<DragonLocomotion>();
        }

        public void HandleAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
        {
            if ( isSprinting == false )
            {
                animator.SetFloat("Horizontal", SnapValue(horizontalMovement), 0.1f, Time.deltaTime);
                animator.SetFloat("Vertical", SnapValue(verticalMovement), 0.1f, Time.deltaTime);
            }
            else
            {
                animator.SetFloat("Horizontal", SnapValue(horizontalMovement) * 2, 0.1f, Time.deltaTime);
                animator.SetFloat("Vertical", SnapValue(verticalMovement) * 2, 0.1f, Time.deltaTime);
            }
        }

        public void PlayTargetAnimation(string targetAnimation, bool isUsingRootMotion, bool canRotate = false)
        {
            animator.applyRootMotion = isUsingRootMotion;
            animator.SetBool("CanRotate", canRotate); // maybe remove 
            animator.SetBool("IsUsingRootMotion", isUsingRootMotion);
            animator.CrossFade(targetAnimation, 0.2f);
        }

        public void HandleUpAndDown(float upDownInput)
        {
            animator.SetFloat("UpDown", SnapValue(upDownInput), 0.1f, Time.deltaTime);
        }

        private void OnAnimatorMove()
        {
            if ( dragonManager.isUsingRootMotion == false )
                return;

            Debug.Log("[Dragon] using Root Motion!");

            float delta = Time.deltaTime;
            dragonLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            dragonLocomotion.rigidbody.velocity = velocity;
        }

        private static float SnapValue(float valueToSnap)
        {
            float snappedValue = 0;
            switch ( valueToSnap )
            {
                case > 0 and < 0.30f :
                    snappedValue = 0.25f;
                    break;
                case > 0.30f and < 0.55f :
                    snappedValue = 0.5f;
                    break;
                case > 0.55f and < 0.80f :
                    snappedValue = 0.75f;
                    break;
                case > 0.80f :
                    snappedValue = 1f;
                    break;
                case < 0 and > -0.30f :
                    snappedValue = -0.25f;
                    break;
                case < -0.30f and > -0.55f :
                    snappedValue = -0.5f;
                    break;
                case < -0.55f and > -0.80f :
                    snappedValue = -0.75f;
                    break;
                case < -0.80f :
                    snappedValue = -1f;
                    break;
                default :
                    snappedValue = 0;
                    break;
            }
            return snappedValue;
        }

    }
}