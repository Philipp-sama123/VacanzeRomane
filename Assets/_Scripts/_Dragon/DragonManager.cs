using UnityEngine;

namespace _Scripts._Dragon {
    public class DragonManager : MonoBehaviour {
        private DragonCamera dragonCamera;
        private DragonInputManager dragonInputManager;
        private DragonLocomotion dragonLocomotionManager;

        public bool isGrounded;
        public bool isInAir;
        public bool isSprinting;
        public bool isUsingRootMotion;

        private void Awake()
        {
            dragonCamera = FindObjectOfType<DragonCamera>();
            dragonInputManager = GetComponent<DragonInputManager>();
            dragonLocomotionManager = GetComponent<DragonLocomotion>();
        }

        private void Update()
        {
            dragonInputManager.HandleAllInputs();
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