using UnityEngine;

namespace _Scripts._Dragon
{
    public class DragonManager : MonoBehaviour
    {
        [Header("Lock On Transform")] public Transform lockOnTransform;
        private DragonInputManager _inputManager;
        private DragonCameraManager _cameraManager;
        private DragonAnimatorManager _animatorManager;
        private DragonLocomotion _playerLocomotion;

        public bool isUsingRootMotion = false;

        [Header("Player Flags")] public bool isInteracting = false;
        public bool isJumping = false;
        public bool isSprinting;
        public bool isGrounded;

        public bool isInAir;
        // public bool canDoCombo;
        // public bool isUsingRightHand;
        // public bool isUsingLeftHand;
        // public bool isInvulnerable;

        private void Awake()
        {
            _inputManager = GetComponent<DragonInputManager>();
            _playerLocomotion = GetComponent<DragonLocomotion>();
            _animatorManager = GetComponentInChildren<DragonAnimatorManager>();
            _cameraManager = FindObjectOfType<DragonCameraManager>();
        }

        private void Update()
        {
            _inputManager.HandleAllInputs();
            _animatorManager.UpdateAnimatorValues(_inputManager.horizontalInput, _inputManager.verticalInput, isSprinting);
            if (_inputManager.jumpInput)
            {
                _inputManager.jumpInput = false;
                _playerLocomotion.HandleJumping();
            }

            if (_inputManager.dodgeInput == true)
            {
                _inputManager.dodgeInput = false;
                // todo make as in dark souls game inputHandler.rollFlag = false;
                // todo make dodging flag -> then handle in Player Manager

                _playerLocomotion.HandleDodge();
            }
        }


        // When you do stuff with a Rigidbody - everything runs much smoother and nicer with fixed Update 
        // Because it gets called each frame per second (?) 
        // - [Unity Specific rule]
        private void FixedUpdate()
        {
            _playerLocomotion.HandleAllMovement();
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;

            if (_cameraManager != null)
                _cameraManager.HandleAllCameraMovement(deltaTime, _inputManager.cameraInputX,
                    _inputManager.cameraInputY);
            else Debug.LogWarning("[Error] No Camera found!");

            isInteracting = _animatorManager.animator.GetBool("IsInteracting");
            isUsingRootMotion = _animatorManager.animator.GetBool("IsUsingRootMotion");
            isJumping = _animatorManager.animator
                .GetBool("IsJumping"); // disable Jumping when it was played - on the animation

            _animatorManager.animator.SetBool("IsGrounded", isGrounded); // Animation transition
        }
    }
}