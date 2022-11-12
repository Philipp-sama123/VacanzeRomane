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
        public bool isUsingRootMotion;

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
            isUsingRootMotion = dragonAnimatorManager.animator.GetBool("IsInteracting"); 
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.deltaTime;
            dragonLocomotionManager.HandleMovement();
            dragonLocomotionManager.HandleRotation(deltaTime);
            dragonLocomotionManager.HandleJumping();
        }

        private void LateUpdate()
        {
            dragonCamera.FollowTarget();
            dragonCamera.RotateCamera(dragonInputManager.horizontalCameraInput, dragonInputManager.verticalCameraInput);
        }
    }
}