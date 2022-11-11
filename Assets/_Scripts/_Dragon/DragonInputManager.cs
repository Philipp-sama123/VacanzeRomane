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
    DragonAnimatorManager animatorManager;

    [Header("Player Movement")]
    public float verticalMovementInput;
    public float horizontalMovementInput;
    private Vector2 movementInput;

    [Header("Camera Rotation")]
    public float verticalCameraInput;
    public float horizontalCameraInput;
    private Vector2 cameraInput;

    private void Awake()
    {
        animatorManager = GetComponent<DragonAnimatorManager>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
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
        //Handle SprintingInput
    }

    private void HandleMovementInput()
    {
        horizontalMovementInput = movementInput.x;
        verticalMovementInput = movementInput.y;
        animatorManager.HandleAnimatorValues(horizontalMovementInput, verticalMovementInput);
    }

    private void HandleCameraInput()
    {
        horizontalCameraInput = cameraInput.x;
        verticalCameraInput = cameraInput.y;
    }
}