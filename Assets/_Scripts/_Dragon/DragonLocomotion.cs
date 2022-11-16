using System.Collections;
using UnityEngine;

namespace _Scripts._Dragon {
    public class DragonLocomotion : MonoBehaviour {
        private Transform cameraObject;

        private DragonAnimatorManager dragonAnimatorManager;
        private DragonInputManager dragonInputHandler;
        private DragonManager dragonManager;

        public Transform myTransform;
        public new Rigidbody rigidbody;
        private Vector3 moveDirection;

        [Header("Ground and Air Detection Stats")]
        [SerializeField] private float groundDetectionRayStartPoint = 1f;
        [SerializeField] private float minimumDistanceNeededToBeginFall = 2f;
        [SerializeField] private float groundDirectionRayDistance = 0.25f;

        [SerializeField] private LayerMask ignoreForGroundCheck;

        [Header("Movement Stats")]
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float walkingSpeed = 1f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float fallingSpeed = 25f;
        [SerializeField] private float leapingVelocity = 2.5f;
        [SerializeField] private float jumpingForce = 10f;

        private float inAirTimer;
        // ToDo: maybe local for Falling
        private Vector3 normalVector;
        private Vector3 targetPosition;

        [Header("Stamina Costs")]
        public float rollStaminaCost = 10;
        public float backStepStaminaCost = 5;
        public float sprintStaminaCost = .1f;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            dragonInputHandler = GetComponent<DragonInputManager>();
            dragonManager = GetComponent<DragonManager>();

            dragonAnimatorManager = GetComponentInChildren<DragonAnimatorManager>();
        }

        private void Start()
        {
            if ( Camera.main != null ) cameraObject = Camera.main.transform;
            else Debug.LogWarning("No Main Camera in the Scene!");

            myTransform = transform;

            dragonManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }

        public void HandleGroundMovement(float deltaTime)
        {
            rigidbody.useGravity = true;

            HandleMovement();
            HandleRotation(deltaTime);
            HandleFalling(deltaTime, moveDirection);

            HandleJumping();
            HandleJumpHold();
        }

        public void HandleFlyingMovement(float deltaTime)
        {
            rigidbody.useGravity = false;

            HandleMovement();
            HandleRotation(deltaTime);
            HandleJumpHold();

        }

        private void HandleRotation(float deltaTime)
        {
            Vector3 targetDir = Vector3.zero;
            targetDir = cameraObject.forward * dragonInputHandler.verticalMovementInput;
            targetDir += cameraObject.right * dragonInputHandler.horizontalMovementInput;
            targetDir.Normalize();
            targetDir.y = 0;

            if ( targetDir == Vector3.zero )
                targetDir = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * deltaTime);

            myTransform.rotation = targetRotation;
        }

        private void HandleMovement()
        {
            moveDirection = cameraObject.forward * dragonInputHandler.verticalMovementInput;
            moveDirection += cameraObject.right * dragonInputHandler.horizontalMovementInput;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if ( dragonInputHandler.sprintFlag && dragonInputHandler.moveAmount > 0.5f )
            {
                speed = sprintSpeed;
                dragonManager.isSprinting = true;
                moveDirection *= speed;
            }
            else
            {
                if ( dragonInputHandler.moveAmount < 0.5f )
                {
                    moveDirection *= walkingSpeed;
                    dragonManager.isSprinting = false;
                }
                else
                {
                    moveDirection *= speed;
                    dragonManager.isSprinting = false;
                }
            }
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            if ( !dragonManager.isUsingRootMotion )
                dragonAnimatorManager.HandleAnimatorValues(0, dragonInputHandler.moveAmount, dragonManager.isSprinting);
        }

        private void HandleFalling(float deltaTime, Vector3 movementDirection)
        {
            dragonManager.isGrounded = false;
            rigidbody.useGravity = true;

            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if ( Physics.Raycast(origin, myTransform.forward, out hit, 0.4f) )
            {
                movementDirection = Vector3.zero;
            }

            if ( dragonManager.isInAir )
            {
                inAirTimer++;
                rigidbody.AddForce(transform.forward * leapingVelocity, ForceMode.Impulse);
                rigidbody.AddForce(Vector3.down * fallingSpeed * 9.8f * inAirTimer * deltaTime, ForceMode.Acceleration);
            }
            Vector3 dir = movementDirection;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red);
            if ( Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck) )
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                dragonManager.isGrounded = true;
                targetPosition.y = tp.y;

                if ( dragonManager.isInAir )
                {
                    if ( inAirTimer > 0.5f )
                    {
                        Debug.Log("[Info] Landing You were in the air for " + inAirTimer);
                        dragonAnimatorManager.PlayTargetAnimation("[Airborne] Landing", false);
                        inAirTimer = 0;
                    }
                    else
                    {
                        Debug.Log("[Info] EMPTY You were in the air for " + inAirTimer);
                        dragonAnimatorManager.PlayTargetAnimation("Empty", false);
                        inAirTimer = 0;
                    }

                    dragonManager.isInAir = false;
                }
            }
            else
            {
                if ( dragonManager.isGrounded )
                {
                    dragonManager.isGrounded = false;
                }

                if ( dragonManager.isInAir == false )
                {
                    if ( dragonManager.isUsingRootMotion == false )
                    {
                        Debug.LogWarning("Falling");
                        if ( dragonManager.isJumping == false )
                            dragonAnimatorManager.PlayTargetAnimation("[Airborne] Falling", true);

                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    dragonManager.isInAir = true;
                }
            }

            if ( dragonInputHandler.moveAmount > 0 )
            {
                Debug.LogWarning("Align Feet");
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, deltaTime / .2f);
            }
            else
            {
                Debug.LogWarning("Align Feet no Movement");
                myTransform.position = targetPosition;
            }
        }

        private void HandleJumping()
        {
            if ( dragonInputHandler.jumpInput )
            {
                dragonInputHandler.jumpInput = false;

                if ( dragonManager.isFlying == false && dragonManager.isGrounded )
                {
                    dragonAnimatorManager.PlayTargetAnimation(dragonInputHandler.moveAmount > 0 ? "Running Jump" : "Standing Jump", false);
                    dragonAnimatorManager.animator.SetBool("IsJumping", true);
                }
            }
        }

        private void HandleJumpHold()
        {
            if ( dragonInputHandler.jumpHoldInput )
            {
                dragonInputHandler.jumpHoldInput = false;

                if ( dragonManager.isFlying == false )
                {
                    dragonAnimatorManager.animator.SetBool("IsFlying", true);
                    dragonAnimatorManager.PlayTargetAnimation("Empty", false);

                }
                else
                {
                    dragonAnimatorManager.animator.SetBool("IsFlying", false);
                    dragonAnimatorManager.PlayTargetAnimation("[Airborne] Falling", false);


                }
            }
        }

        /**
        * Animator Events
        **/
        #region Animator Events

        public void AddJumpingForce()
        {
            float forceMultiplier = 10f;

            if ( dragonInputHandler.moveAmount > 0 )
            {
                rigidbody.AddForce(Vector3.forward * dragonInputHandler.moveAmount, ForceMode.Impulse);
            }

            rigidbody.AddForce(Vector3.up * forceMultiplier * jumpingForce, ForceMode.Impulse);
        }

        #endregion

    }
}