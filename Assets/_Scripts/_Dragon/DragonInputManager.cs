using UnityEngine;

namespace _Scripts._Dragon {
    public class DragonInputManager : MonoBehaviour {
        private PlayerControls playerControls;
        public float horizontalMovementInput { get; private set; }
        public float verticalMovementInput { get; private set; }
        public float horizontalCameraInput { get; private set; }
        public float verticalCameraInput { get; private set; }
        public float moveAmount { get; private set; }

        private Vector2 movementInput;
        private Vector2 cameraInput;

        [Header("Camera Rotation")]
        public bool sprintFlag;
        public bool jumpInput;

        private void OnEnable()
        {
            if ( playerControls == null )
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
                // playerControls.PlayerActions.Jump.canceled += i => jumpInput = false;
                playerControls.PlayerActions.Roll.performed += i => sprintFlag = true;
                playerControls.PlayerActions.Roll.canceled += i => sprintFlag = false;
            }
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        public void HandleAllInputs()
        {
            HandleMovementInput();
            HandleCameraInput();
        }

        private void HandleMovementInput()
        {
            horizontalMovementInput = movementInput.x;
            verticalMovementInput = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalMovementInput) + Mathf.Abs(verticalMovementInput));
        }

        private void HandleCameraInput()
        {
            horizontalCameraInput = cameraInput.x;
            verticalCameraInput = cameraInput.y;
        }
    }
}