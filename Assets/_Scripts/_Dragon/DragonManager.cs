using UnityEngine;

namespace _Scripts._Dragon {
    public class DragonManager : MonoBehaviour {
        DragonCamera playerCamera;
        DragonInputManager inputManager;
        DragonLocomotion playerLocomotionManager;

        private void Awake()
        {
            playerCamera = FindObjectOfType<DragonCamera>();
            inputManager = GetComponent<DragonInputManager>();
            playerLocomotionManager = GetComponent<DragonLocomotion>();
        }

        private void Update()
        {
            inputManager.HandleAllInputs();
        }

        private void FixedUpdate()
        {
            playerLocomotionManager.HandleAllLocomotion();
        }

        private void LateUpdate()
        {
            playerCamera.HandleAllCameraMovement();
        }
    }
}