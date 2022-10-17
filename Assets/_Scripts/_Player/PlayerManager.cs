using System;
using _Scripts._UI;
using UnityEngine;

namespace _Scripts._Player {
    public class PlayerManager : CharacterManager {

        private InputHandler inputHandler;
        private Animator animator;
        private CameraHandler cameraHandler;
        private PlayerStats playerStats;
        private PlayerLocomotion playerLocomotion;
        private PlayerAnimatorManager playerAnimatorManager;

        private InteractableUI interactableUI;
        public GameObject interactableUIGameObject;
        public GameObject itemInteractableGameObject;

        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;
        public bool isUsingRightHand;
        public bool isUsingLeftHand;
        public bool isInvulnerable;

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
            interactableUI = FindObjectOfType<InteractableUI>();

            backStabCollider = GetComponentInChildren<CriticalDamageCollider>();
            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
            animator = GetComponentInChildren<Animator>();

            playerLocomotion = GetComponent<PlayerLocomotion>();
            inputHandler = GetComponent<InputHandler>();
            playerStats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            if ( inputHandler == null || animator == null ) return;

            isUsingRightHand = animator.GetBool("IsUsingRightHand");
            isUsingLeftHand = animator.GetBool("IsUsingLeftHand");
            isInvulnerable = animator.GetBool("IsInvulnerable");
            isInteracting = animator.GetBool("IsInteracting");
            canDoCombo = animator.GetBool("CanDoCombo");
            isFiringSpell = animator.GetBool("IsFiringSpell");

            // Handled by Scripts
            animator.SetBool("IsInAir", isInAir);
            animator.SetBool("IsBlocking", isBlocking);
            animator.SetBool("IsDead", playerStats.isDead);

            playerAnimatorManager.canRotate = animator.GetBool("CanRotate");
            inputHandler.TickInput(deltaTime);

            #region RootMotion Movement

            // those are playing a Root Motion Animation so it NEEDS to be in Update to not be out of sync
            playerLocomotion.HandleRollingAndSprinting();
            playerLocomotion.HandleJumping(); // playerLocomotion.HandleJumping();

            #endregion

            playerStats.RegenerateStamina();
            CheckForInteractableObject();
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;

            // TODO: explain why?  --> there you go: those are Functions which are here to control the Rigidbody so they need to be here regarding the docs
            playerLocomotion.HandleMovement(deltaTime);
            playerLocomotion.HandleRotation(deltaTime);
            playerLocomotion.HandleFalling(deltaTime, playerLocomotion.moveDirection);
        }

        private void LateUpdate()
        {
            inputHandler.rollFlag = false;

            inputHandler.rbInput = false;
            inputHandler.rtInput = false;
            inputHandler.jumpInput = false;
            inputHandler.ltInput = false;

            inputHandler.dPadRight = false;
            inputHandler.dPadLeft = false;
            inputHandler.dPadUp = false;
            inputHandler.dPadDown = false;

            inputHandler.aInput = false;
            inputHandler.inventoryInput = false;

            if ( cameraHandler != null )
            {
                // Frame Rate independent, so deltaTime is not required
                cameraHandler.FollowTarget();
                cameraHandler.HandleCameraRotation(inputHandler.mouseX, inputHandler.mouseY);
            }
            else
            {
                Debug.LogWarning("Please add a Camera to your Scene");
            }

            if ( isInAir )
            {
                playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
            }
        }

        private void CheckForInteractableObject()
        {
            RaycastHit hit;

            if ( Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers) )
            {
                if ( hit.collider.CompareTag("Interactable") )
                {
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();
                    if ( interactableObject != null )
                    {
                        string interactableText = interactableObject.interactableText;
                        interactableUI.interactableText.text = interactableText;
                        interactableUIGameObject.SetActive(true);
                        if ( inputHandler.aInput )
                        {
                            hit.collider.GetComponent<Interactable>().Interact(this);
                        }
                    }
                }
            }
            else
            {
                if ( interactableUIGameObject != null )
                {
                    interactableUIGameObject.SetActive(false);
                }
                if ( itemInteractableGameObject != null && inputHandler.aInput )
                {
                    itemInteractableGameObject.SetActive(false);
                }
            }
        }

        public void OpenChestInteraction(Transform playerStandingPosition)
        {
            playerLocomotion.rigidbody.velocity = Vector3.zero;
            transform.position = playerStandingPosition.transform.position;
            playerAnimatorManager.PlayTargetAnimation("[Action] Open Chest", true);
        }
    }
}