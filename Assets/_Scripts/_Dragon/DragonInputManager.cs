using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts._Dragon;
using _Scripts._Player;
using UnityEngine;

public class DragonInputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private DragonManager playerManager;
    private DragonCameraManager cameraManager;

    public Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput;

    public Vector2 cameraInput;
    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;

    public bool sprintInput;
    public bool jumpInput;
    public bool dodgeInput;

    /** Lock On **/
    public bool lockOnInput;
    public bool lockOnFlag;
    public bool rightStickLeftInput;
    public bool rightStickRightInput;

    private void Awake()
    {
        // todo: just import player manager
        playerManager = GetComponent<DragonManager>();
        cameraManager = FindObjectOfType<DragonCameraManager>();
    }

    private void OnEnable()
    {
        if ( playerControls == null )
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            // while you hold it --> true!
            playerControls.PlayerActions.Roll.performed += i => sprintInput = true;
            playerControls.PlayerActions.Roll.canceled += i => sprintInput = false;
            // when you press the button --> True
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            // playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
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
        HandleSprintingInput();
        HandleLockOnInput(); 
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
    }

    private void HandleSprintingInput()
    {
        if ( sprintInput && moveAmount > 0.5f )
        {
            playerManager.isSprinting = true;
        }
        else
        {
            playerManager.isSprinting = false;
        }
    }

    private void HandleLockOnInput()
    {
        if ( lockOnInput && lockOnFlag == false )
        {
            lockOnInput = false;
            cameraManager.HandleLockOn();

            if ( cameraManager.nearestLockOnTarget != null )
            {
                cameraManager.currentLockOnTarget = cameraManager.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        else if ( lockOnInput && lockOnFlag )
        {
            lockOnInput = false;
            lockOnFlag = false;
            cameraManager.ClearLockOnTargets();
        }

        if ( lockOnFlag && rightStickLeftInput )
        {
            rightStickLeftInput = false;
            cameraManager.HandleLockOn();

            if ( cameraManager.leftLockTarget != null )
            {
                cameraManager.currentLockOnTarget = cameraManager.leftLockTarget;
            }
        }

        if ( lockOnFlag && rightStickRightInput )
        {
            rightStickRightInput = false;
            cameraManager.HandleLockOn();

            if ( cameraManager.rightLockTarget != null )
            {
                cameraManager.currentLockOnTarget = cameraManager.rightLockTarget;
            }
        }
        
        cameraManager.SetCameraHeight();
    }
}