using System.Collections.Generic;
using _Scripts._Player;
using UnityEngine;

namespace _Scripts._Dragon {
    public class DragonCamera : MonoBehaviour {
        public DragonInputManager inputManager;

        public Transform cameraPivot;
        public Camera cameraObject;
        public GameObject player;

        Vector3 cameraFollowVelocity = Vector3.zero;
        Vector3 targetPosition;
        Vector3 cameraRotation;
        Quaternion targetRotation;

        [Header("Camera Speeds")]
        public float cameraSmoothTime = 0.2f;

        float lookAmountVertical;
        float lookAmountHorizontal;
        float maximumPivotAngle = 15;
        float minimumPivotAngle = -15;

        public void HandleAllCameraMovement()
        {
            FollowPlayer();
            RotateCamera();
        }

        private void FollowPlayer()
        {
            targetPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraFollowVelocity, cameraSmoothTime * Time.deltaTime);
            transform.position = targetPosition;
        }

        private void RotateCamera()
        {
            lookAmountVertical = lookAmountVertical + (inputManager.horizontalCameraInput);
            lookAmountHorizontal = lookAmountHorizontal - (inputManager.verticalCameraInput);
            lookAmountHorizontal = Mathf.Clamp(lookAmountHorizontal, minimumPivotAngle, maximumPivotAngle);

            cameraRotation = Vector3.zero;
            cameraRotation.y = lookAmountVertical;
            targetRotation = Quaternion.Euler(cameraRotation);
            targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraSmoothTime);
            transform.rotation = targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.x = lookAmountHorizontal;
            targetRotation = Quaternion.Euler(cameraRotation);
            targetRotation = Quaternion.Slerp(cameraPivot.localRotation, targetRotation, cameraSmoothTime);
            cameraPivot.localRotation = targetRotation;
        }
    }
}