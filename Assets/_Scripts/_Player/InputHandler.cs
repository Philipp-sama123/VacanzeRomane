using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts._Player {
    public class InputHandler : MonoBehaviour {
        [Header("Movement and Camera Input")]
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        [Header("Quickslot Inputs")]
        public bool dPadUp;
        public bool dPadDown;
        public bool dPadLeft;
        public bool dPadRight;

        public float rollInputTimer;
        public bool rollAndSprintInput;
        public bool jumpInput;

        public bool aInput;
        public bool twoHandInput;
        public bool inventoryInput;

        public bool lockOnInput;
        public bool rightStickRightInput;
        public bool rightStickLeftInput;

        public bool rbInput;
        public bool rtInput;
        public bool ltInput;
        public bool lbInput;

        public bool criticalAttackInput;

        public bool rollFlag;
        public bool twoHandFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool inventoryFlag;

        public bool lockOnFlag;

        private PlayerAttacker playerAttacker;
        private WeaponSlotManager weaponSlotManager;
        private PlayerInventory playerInventory;

        private PlayerAnimatorManager playerAnimatorManager;
        private PlayerControls inputActions;
        private PlayerManager playerManager;
        private PlayerStats playerStats;

        private BlockingCollider blockingCollider;

        private CameraHandler cameraHandler;
        private UIManager uiManager;

        private Vector2 movementInput;
        private Vector2 cameraInput;
        public Transform criticalAttackRaycastStartPoint;

        private void Awake()
        {
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            playerStats = GetComponentInChildren<PlayerStats>();

            playerAttacker = GetComponentInChildren<PlayerAttacker>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();

            blockingCollider = GetComponentInChildren<BlockingCollider>();

            uiManager = FindObjectOfType<UIManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        private void OnEnable()
        {
            if ( inputActions == null )
            {
                inputActions = new PlayerControls();

                inputActions.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                inputActions.PlayerActions.RT.performed += i => rtInput = true;
                inputActions.PlayerActions.RB.performed += i => rbInput = true;

                inputActions.PlayerActions.LT.performed += i => ltInput = true;

                inputActions.PlayerActions.LB.performed += i => lbInput = true;
                inputActions.PlayerActions.LB.canceled += i => lbInput = false;

                inputActions.PlayerActions.Roll.performed += i => rollAndSprintInput = true;
                inputActions.PlayerActions.Roll.canceled += i => rollAndSprintInput = false; // when you unpress - changes the bool to false

                inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
                inputActions.PlayerActions.TwoHanded.performed += i => twoHandInput = true;

                inputActions.PlayerMovement.LockOnTargetRight.performed += i => rightStickRightInput = true;
                inputActions.PlayerMovement.LockOnTargetLeft.performed += i => rightStickLeftInput = true;

                inputActions.PlayerQuickslots.DPadRight.performed += i => dPadRight = true;
                inputActions.PlayerQuickslots.DPadLeft.performed += i => dPadLeft = true;
                inputActions.PlayerQuickslots.DPadUp.performed += i => dPadUp = true;
                inputActions.PlayerQuickslots.DPadDown.performed += i => dPadDown = true;

                // ToDo: find another input (currently tap on light attack) 
                inputActions.PlayerActions.CriticalAttack.performed += i => criticalAttackInput = true;
                inputActions.PlayerActions.Inventory.performed += i => inventoryInput = true;
                inputActions.PlayerActions.Jump.performed += i => jumpInput = true;
                inputActions.PlayerActions.A.performed += i => aInput = true;
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float deltaTime)
        {
            HandleMoveInput(deltaTime);
            HandleRollAndSprintInput(deltaTime);
            HandleCombatInput(deltaTime);
            HandleQuickSlotsInput();
            HandleInventoryInput();
            HandleLockOnInput();
            HandleTwoHandInput();
            HandleCriticalAttackInput();
        }

        private void HandleMoveInput(float deltaTime)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollAndSprintInput(float deltaTime)
        {
            if ( rollAndSprintInput )
            {
                rollInputTimer += deltaTime;
                if ( playerStats.currentStamina <= 0 )
                {
                    rollAndSprintInput = false;
                    sprintFlag = false;
                }

                if ( moveAmount > 0.5f && playerStats.currentStamina > 0 )
                {
                    sprintFlag = true;
                }
            }
            else
            {
                sprintFlag = false;
                if ( rollInputTimer > 0 && rollInputTimer < 0.5f )
                {
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }
        }

        private void HandleCombatInput(float deltaTime)
        {
            if ( rbInput )
            {
                playerAttacker.HandleRbInput();
            }
            if ( lbInput )
            {
                playerAttacker.HandleLbAction();
            }
            else
            {
                playerManager.isBlocking = false;
                if ( blockingCollider.blockingBoxCollider.enabled )
                {
                    blockingCollider.blockingBoxCollider.enabled = false;
                }
            }

            if ( rtInput )
            {
                if ( playerManager.canDoCombo )
                {
                    comboFlag = true;
                    playerAnimatorManager.animator.SetBool("IsUsingRightHand", true); // Todo: think of removing it here - but where (?) -oo-> Animator handler + appropriate fct. 
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                }
                else
                {
                    if ( playerManager.isInteracting || playerManager.canDoCombo ) return;
                    playerAnimatorManager.animator.SetBool("IsUsingRightHand", true); // Todo: think of removing it here - but where (?) -oo-> Animator handler + appropriate fct. 
                    playerAttacker.HandleRtInput(playerInventory.rightWeapon);
                }
            }

            if ( ltInput )
            {
                if ( twoHandFlag )
                {

                    // if two handed --> Handle weapon art
                }
                else
                {
                    playerAttacker.HandleLtAction();
                }
                // if shield --> Handle weapon art
                // else if melee --> Handle secondary Attack
            }
        }

        private void HandleQuickSlotsInput()
        {
            if ( dPadRight )
            {
                playerInventory.ChangeRightWeapon();
            }
            else if ( dPadLeft )
            {
                playerInventory.ChangeLeftWeapon();
            }
        }

        private void HandleInventoryInput()
        {

            if ( inventoryInput )
            {
                inventoryFlag = !inventoryFlag;

                if ( inventoryFlag )
                {
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI();
                    uiManager.hudWindow.SetActive(false);
                }
                else
                {
                    uiManager.CloseSelectWindow();
                    uiManager.hudWindow.SetActive(true);

                    uiManager.CloseAllInventoryWindows();
                }
            }
        }

        private void HandleLockOnInput()
        {
            if ( lockOnInput && lockOnFlag == false )
            {
                lockOnInput = false;
                cameraHandler.HandleLockOn();

                if ( cameraHandler.nearestLockOnTarget != null )
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    lockOnFlag = true;
                }
            }
            else if ( lockOnInput && lockOnFlag )
            {
                lockOnInput = false;
                lockOnFlag = false;
                cameraHandler.ClearLockOnTargets();
            }

            if ( lockOnFlag && rightStickLeftInput )
            {
                rightStickLeftInput = false;
                cameraHandler.HandleLockOn();

                if ( cameraHandler.leftLockTarget != null )
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                }
            }

            if ( lockOnFlag && rightStickRightInput )
            {
                rightStickRightInput = false;
                cameraHandler.HandleLockOn();

                if ( cameraHandler.rightLockTarget != null )
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
                }
            }
            cameraHandler.SetCameraHeight();
        }

        private void HandleTwoHandInput()
        {
            if ( twoHandInput )
            {
                twoHandInput = false;
                twoHandFlag = !twoHandFlag;
                if ( twoHandFlag )
                {
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                }
                else
                {
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);
                }
            }
        }

        private void HandleCriticalAttackInput()
        {
            if ( criticalAttackInput )
            {
                criticalAttackInput = false;
                playerAttacker.AttemptBackStabOrRiposte();
            }
        }
    }
}