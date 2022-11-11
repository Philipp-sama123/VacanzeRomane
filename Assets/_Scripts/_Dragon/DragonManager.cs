using UnityEngine;

namespace _Scripts._Dragon {
    public class DragonManager : MonoBehaviour {
        DragonCamera playerCamera;
        DragonInputManager inputManager;
        DragonLocomotion playerLocomotionManager;

        public bool isGrounded;
        public bool isInAir;
        public bool isSprinting;
        public bool isUsingRootMotion;

        private void Awake()
        {
            playerCamera = FindObjectOfType<DragonCamera>();
            inputManager = GetComponent<DragonInputManager>();
            playerLocomotionManager = GetComponent<DragonLocomotion>();
        }

        private void Update()
        {
            inputManager.HandleAllInputs();
            playerLocomotionManager.HandleJumping();
        }

        private void FixedUpdate()
        {

            var deltaTime = Time.deltaTime;
            playerLocomotionManager.HandleMovement();
            playerLocomotionManager.HandleRotation(deltaTime);
        }

        private void LateUpdate()
        {
            playerCamera.FollowTarget();
            playerCamera.RotateCamera(inputManager.horizontalCameraInput,inputManager.verticalCameraInput);
        }
    }
}