using System.Collections;
using UnityEngine;

namespace _Scripts._Player {
    public class PlayerLocomotion : MonoBehaviour {
        private Transform cameraObject;
        private CameraHandler cameraHandler;

        private PlayerAnimatorManager playerAnimatorManager;
        private PlayerManager playerManager;
        private InputHandler inputHandler;
        private PlayerStats playerStats;

        public Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        public new Rigidbody rigidbody;

        [Header("Ground and Air Detection Stats")]
        [SerializeField] private float groundDetectionRayStartPoint = 0.5f;
        [SerializeField] private float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField] private float groundDirectionRayDistance = 0.25f;

        public float inAirTimer;
        [SerializeField] private LayerMask ignoreForGroundCheck;

        [Header("Movement Stats")]
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float walkingSpeed = 1f;
        [SerializeField] float rotationSpeed = 10f;
        [SerializeField] float fallingSpeed = 45f;
        [SerializeField] float leapingVelocity = 2.5f;
        [SerializeField] private float jumpForce = 2f;
        [SerializeField] private float jumpAccelerationDuration = 0.2f;

        [Header("Stamina Costs")]
        [SerializeField] private float rollStaminaCost = 10;
        [SerializeField] private float backStepStaminaCost = 5;
        [SerializeField] private float sprintStaminaCost = .1f;

        [SerializeField] private CapsuleCollider characterCollider;
        [SerializeField] private CapsuleCollider characterCollisionBlockerCollider;

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();

            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            playerManager = GetComponent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();

            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
        }

        private void Start()
        {
            if ( Camera.main != null ) cameraObject = Camera.main.transform;
            else Debug.LogWarning("No Main Camera in the Scene!");

            myTransform = transform;
            playerAnimatorManager.Initialize();

            playerManager.isGrounded = true;

            IgnoreCharacterColliderCollision();
        }

        #region Movement

        private Vector3 normalVector;
        private Vector3 targetPosition;

        public void HandleRotation(float deltaTime)
        {
            if ( !playerAnimatorManager.canRotate ) return;

            if ( inputHandler.lockOnFlag )
            {
                if ( inputHandler.sprintFlag || inputHandler.rollFlag )
                {
                    Vector3 targetDir = Vector3.zero;
                    targetDir = cameraHandler.cameraTransform.forward * inputHandler.vertical;
                    targetDir += cameraHandler.cameraTransform.right * inputHandler.horizontal;

                    targetDir.Normalize();
                    targetDir.y = 0;

                    if ( targetDir == Vector3.zero )
                    {
                        targetDir = transform.forward;
                    }

                    Quaternion tr = Quaternion.LookRotation(targetDir);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                    transform.rotation = targetRotation;
                }
                else
                {
                    Vector3 rotationDirection = moveDirection;
                    rotationDirection = cameraHandler.currentLockOnTarget.transform.position - transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();

                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
            }
            else
            {
                Vector3 targetDir = Vector3.zero;
                float movementOverride = inputHandler.moveAmount;

                targetDir = cameraObject.forward * inputHandler.vertical;
                targetDir += cameraObject.right * inputHandler.horizontal;

                targetDir.Normalize();
                targetDir.y = 0; // no movement on y-Axis (!)

                if ( targetDir == Vector3.zero )
                    targetDir = myTransform.forward;

                float rs = rotationSpeed;

                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * deltaTime);

                myTransform.rotation = targetRotation;
            }
        }

        public void HandleMovement(float deltaTime)
        {
            if ( inputHandler.rollFlag ) return;
            if ( playerManager.isInteracting ) return;

            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if ( inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f )
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
                playerStats.TakeStaminaDamage(sprintStaminaCost);
            }
            else
            {
                if ( inputHandler.moveAmount < 0.5f )
                {
                    moveDirection *= walkingSpeed;
                    playerManager.isSprinting = false;
                }
                else
                {
                    moveDirection *= speed;
                    playerManager.isSprinting = false;
                }
            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            if ( inputHandler.lockOnFlag && inputHandler.sprintFlag == false )
            {
                playerAnimatorManager.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);

            }
            else
            {
                playerAnimatorManager.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
            }
        }

        public void HandleRollingAndSprinting()
        {
            if ( playerManager.isInteracting )
            {
                return;
            }

            if ( playerStats.currentStamina <= 0 )
            {
                return;
            }

            if ( inputHandler.rollFlag )
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if ( inputHandler.moveAmount > 0 )
                {
                    playerAnimatorManager.PlayTargetAnimation("Rolling Forward", true); // Todo: rename Anim
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                    playerStats.TakeStaminaDamage(rollStaminaCost);
                }
                else
                {
                    playerAnimatorManager.PlayTargetAnimation("Dodge Backward", true); // Todo: rename Anim
                    moveDirection.y = 0;
                    playerStats.TakeStaminaDamage(backStepStaminaCost);

                }
            }

            moveDirection = cameraObject.forward * inputHandler.vertical;
        }

        //TODO adjust values dep on Character
        //TODO IsInteracting in animator Manager

        public void HandleFalling(float deltaTime, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if ( Physics.Raycast(origin, myTransform.forward, out hit, 0.4f) )
            {
                moveDirection = Vector3.zero;
            }

            if ( playerManager.isInAir )
            {
                rigidbody.AddForce(transform.forward * leapingVelocity, ForceMode.Impulse);
                rigidbody.AddForce(-Vector3.up * fallingSpeed * 9.8f * inAirTimer, ForceMode.Acceleration);
            }
            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red);
            if ( Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck) )
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                if ( playerManager.isInAir )
                {
                    if ( inAirTimer > 0.5f )
                    {
                        Debug.Log("[Info] You were in the air for " + inAirTimer);
                        playerAnimatorManager.PlayTargetAnimation("[Airborne] Landing", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        playerAnimatorManager.PlayTargetAnimation("Empty", false);
                        inAirTimer = 0;
                    }

                    playerManager.isInAir = false;
                }
            }
            else
            {
                if ( playerManager.isGrounded )
                {
                    playerManager.isGrounded = false;
                }

                if ( playerManager.isInAir == false )
                {
                    if ( playerManager.isInteracting == false )
                    {
                        playerAnimatorManager.PlayTargetAnimation("[Airborne] Falling", true, true);
                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
            }

            // ToDo: put in update ! because it makes the character jittery in fixed update -> Transform manipulation vs Rigidbody
            // maybe calculate velocity required to put the player up

            if ( playerManager.isInteracting || inputHandler.moveAmount > 0 )
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, deltaTime / 0.1f);
            }
            else
            {
                myTransform.position = targetPosition;
            }

        }

        public void HandleJumpingAlternative()
        {
            if ( playerManager.isInteracting ) return;
            if ( playerStats.currentStamina <= 0 ) return;

            if ( inputHandler.jumpInput )
            {
                playerAnimatorManager.PlayTargetAnimation("[Airborne] Jump Start", true, true);
                // rigidbody.AddForce(Vector3.up * jumpForce* 10, ForceMode.Impulse);
            }
        }

        #endregion

        private void IgnoreCharacterColliderCollision()
        {
            if ( characterCollider == null )
            {
                Debug.LogWarning("[Action required] assign the character in the Unity-UI");
                return;
            }
            if ( characterCollisionBlockerCollider == null )
            {
                Debug.LogWarning("[Action required] assign the characterCollisionBlocker in the Unity-UI");
                return;
            }
            Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);
        }
    }
}