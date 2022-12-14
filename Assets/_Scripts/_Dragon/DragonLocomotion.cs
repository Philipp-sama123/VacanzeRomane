using System.Collections;
using UnityEngine;

namespace _Scripts._Dragon
{
    public class DragonLocomotion : MonoBehaviour
    {
        private Transform cameraObject;

        private DragonAnimatorManager dragonAnimatorManager;
        private DragonInputManager dragonInputManager;
        private DragonManager dragonManager;

        public Transform myTransform;
        public new Rigidbody rigidbody;
        private Vector3 moveDirection;

        [Header("Ground and Air Detection Stats")] [SerializeField]
        private float groundDetectionRayStartPoint = 1f;

        [SerializeField] private float minimumDistanceNeededToBeginFall = 2f;
        [SerializeField] private float groundDirectionRayDistance = 0.25f;

        [SerializeField] private LayerMask ignoreForGroundCheck;

        [Header("Movement Stats")] [SerializeField]
        private float movementSpeed = 5f;

        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float walkingSpeed = 1f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float fallingSpeed = 25f;
        [SerializeField] private float leapingVelocity = 2.5f;
        [SerializeField] private float jumpingForce = 10f;
        [SerializeField] private float upDownForce = 2f;

        private float inAirTimer;

        // ToDo: maybe local for Falling
        private Vector3 normalVector;
        private Vector3 targetPosition;

        [Header("Stamina Costs")] public float rollStaminaCost = 10;
        public float backStepStaminaCost = 5;
        public float sprintStaminaCost = .1f;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            dragonInputManager = GetComponent<DragonInputManager>();
            dragonManager = GetComponent<DragonManager>();

            dragonAnimatorManager = GetComponentInChildren<DragonAnimatorManager>();
        }

        private void Start()
        {
            if (Camera.main != null) cameraObject = Camera.main.transform;
            else Debug.LogWarning("No Main Camera in the Scene!");

            myTransform = transform;

            dragonManager.isGrounded = true;
        }

        public void HandleGroundMovement(float deltaTime)
        {
            rigidbody.useGravity = true;

            if (dragonManager.isGrounded)
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
            HandleFlyingGroundCheck();

            HandleJumpHold();
            HandleAirControl();
        }

        private void HandleFlyingGroundCheck()
        {
            dragonManager.isGrounded = false;

            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            Vector3 dir = Vector3.zero;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red);
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                dragonManager.isGrounded = true;
                targetPosition.y = tp.y;

                if (dragonManager.isInAir)
                {
                    Debug.Log("[Info] Landing You were in the air for " + inAirTimer);
                    dragonAnimatorManager.PlayTargetAnimation("[Airborne] Landing", false);
                    inAirTimer = 0;
                    dragonAnimatorManager.animator.SetBool("IsFlying", false);
                    dragonManager.isInAir = false;
                }
            }
        }

        private void HandleAirControl()
        {
            Vector3 airForce = cameraObject.forward * dragonInputManager.verticalMovementInput;
            airForce += cameraObject.right * dragonInputManager.horizontalMovementInput;
            // go up
            airForce += cameraObject.up * dragonInputManager.upDownInput * upDownForce;

            Vector3 projectedVelocity = Vector3.Project(airForce, normalVector);
            if (dragonInputManager.sprintFlag)
                projectedVelocity *= 2;
            rigidbody.velocity += projectedVelocity;

            if (!dragonManager.isUsingRootMotion)
                dragonAnimatorManager.HandleUpAndDown(dragonInputManager.upDownInput);
        }

        private void HandleRotation(float deltaTime)
        {
            Vector3 targetDir = Vector3.zero;
            targetDir = cameraObject.forward * dragonInputManager.verticalMovementInput;
            targetDir += cameraObject.right * dragonInputManager.horizontalMovementInput;
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * deltaTime);

            myTransform.rotation = targetRotation;
        }

        private void HandleMovement()
        {
            moveDirection = cameraObject.forward * dragonInputManager.verticalMovementInput;
            moveDirection += cameraObject.right * dragonInputManager.horizontalMovementInput;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if (dragonInputManager.sprintFlag && dragonInputManager.moveAmount > 0.75f)
            {
                speed = sprintSpeed;
                dragonManager.isSprinting = true;
                moveDirection *= speed;
            }
            else
            {
                if (dragonInputManager.moveAmount < 0.75f)
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

            if (!dragonManager.isUsingRootMotion)
                dragonAnimatorManager.HandleAnimatorValues(0, dragonInputManager.moveAmount, dragonManager.isSprinting);
        }

        private void HandleFalling(float deltaTime, Vector3 movementDirection)
        {
            dragonManager.isGrounded = false;
            rigidbody.useGravity = true;

            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                movementDirection = Vector3.zero;
            }

            if (dragonManager.isInAir)
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
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                dragonManager.isGrounded = true;
                targetPosition.y = tp.y;

                if (dragonManager.isInAir)
                {
                    if (inAirTimer > 0.5f)
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
                if (dragonManager.isGrounded)
                {
                    dragonManager.isGrounded = false;
                }

                if (dragonManager.isInAir == false)
                {
                    if (dragonManager.isUsingRootMotion == false)
                    {
                        Debug.LogWarning("Falling");
                        dragonAnimatorManager.PlayTargetAnimation("[Airborne] Falling", true);
                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    dragonManager.isInAir = true;
                }
            }

            if (dragonInputManager.moveAmount > 0)
            {
                Debug.LogWarning("Align Feet");
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, deltaTime / .2f);
            }
            else
            {
                Debug.LogWarning("Align Feet no Movement");
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, deltaTime / .2f);
            }
        }

        private void HandleJumping()
        {
            if (dragonInputManager.jumpInput)
            {
                dragonInputManager.jumpInput = false;

                if (dragonManager.isFlying == false && dragonManager.isGrounded )
                {
                    
                    if(dragonInputManager.moveAmount > .5f)
                    {
                        dragonAnimatorManager.PlayTargetAnimation("Running Jump", true);
                    }
                    else
                    {
                        dragonAnimatorManager.PlayTargetAnimation("Standing Jump", true);

                    }
                }
            }
        }

        private void HandleJumpHold()
        {
            if (dragonInputManager.jumpHoldInput)
            {
                dragonInputManager.jumpHoldInput = false;

                if (dragonManager.isFlying == false)
                {
                    dragonAnimatorManager.animator.SetBool("IsFlying", true);
                    dragonAnimatorManager.PlayTargetAnimation("Empty", false);
                }
                else
                {
                    dragonAnimatorManager.animator.SetBool("IsFlying", false);
                    dragonAnimatorManager.PlayTargetAnimation("[Airborne] Falling", true);
                }
            }
        }

      
    }
}