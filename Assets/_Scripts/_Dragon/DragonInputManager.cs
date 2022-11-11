using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts._Dragon;
using _Scripts._Player;
using UnityEngine;

public class DragonInputManager : MonoBehaviour
{
    PlayerControls playerControls;

    [Header("Player Movement")]
    public float verticalMovementInput;
    public float horizontalMovementInput;
    public float moveAmount;
    
    private Vector2 movementInput;
    private Vector2 cameraInput;


    [Header("Camera Rotation")]
    public float horizontalCameraInput; 
    public float verticalCameraInput; 
    
    public bool sprintFlag;
    public bool jumpInput;

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
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
        // HandleSprintingInput(); 
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