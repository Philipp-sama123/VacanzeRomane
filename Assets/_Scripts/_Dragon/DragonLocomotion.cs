using UnityEngine;

namespace _Scripts._Dragon {
    public class DragonLocomotion : MonoBehaviour {
        DragonInputManager inputManager;

        [Header("Camera Transform")]
        public Transform playerCamera;

        [Header("Movement Speed")]
        public float rotationSpeed = 3.5f;

        [Header("Rotation Varaibles")]
        Quaternion targetRotation; //The place we want to rotate
        Quaternion playerRotation; //The place we are rotating now, constantly changing

        private void Awake()
        {
            inputManager = GetComponent<DragonInputManager>();
        }

        public void HandleAllLocomotion()
        {
            HandleRotation();
            //HandleFalling();
        }

        private void HandleRotation()
        {
            targetRotation = Quaternion.Euler(0, playerCamera.eulerAngles.y, 0);
            playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if ( inputManager.verticalMovementInput != 0 || inputManager.horizontalMovementInput != 0 )
            {
                transform.rotation = playerRotation;
            }
        }
    }
}