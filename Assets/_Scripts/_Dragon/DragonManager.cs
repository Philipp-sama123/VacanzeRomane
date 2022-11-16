using UnityEngine;

namespace _Scripts._Dragon {
    public class DragonManager : MonoBehaviour {

        private DragonCamera dragonCamera;
        private DragonInputManager dragonInputManager;
        private DragonLocomotion dragonLocomotionManager;
        private DragonAnimatorManager dragonAnimatorManager;

        public bool isGrounded;
        public bool isInAir;
        public bool isSprinting;
        public bool isJumping;
        public bool isUsingRootMotion;
        public bool isFlying;

        private void Awake()
        {
            dragonCamera = FindObjectOfType<DragonCamera>();
            dragonInputManager = GetComponent<DragonInputManager>();
            dragonLocomotionManager = GetComponent<DragonLocomotion>();
            dragonAnimatorManager = GetComponent<DragonAnimatorManager>();
        }

        private void Update()
        {
            dragonInputManager.HandleAllInputs();
            UpdateStateBoolValuesFromAnimator();
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.deltaTime;

            if ( isFlying )
            {
                dragonLocomotionManager.HandleFlyingMovement(deltaTime);
            }
            else
            {
                dragonLocomotionManager.HandleGroundMovement(deltaTime);
            }
        }

        private void LateUpdate()
        {
            dragonCamera.FollowTarget();
            dragonCamera.RotateCamera(dragonInputManager.horizontalCameraInput, dragonInputManager.verticalCameraInput);
        }

        private void UpdateStateBoolValuesFromAnimator()
        {
            isFlying = dragonAnimatorManager.animator.GetBool("IsFlying");
            isJumping = dragonAnimatorManager.animator.GetBool("IsJumping");
            isUsingRootMotion = dragonAnimatorManager.animator.GetBool("IsUsingRootMotion");
        }

    }

}