using UnityEngine;

namespace _Scripts._Dragon {
    public class DragonLocomotion : MonoBehaviour {
        private Transform cameraObject;
        private DragonCamera cameraHandler;

        private DragonAnimatorManager playerAnimatorManager;
        private DragonManager playerManager;
        private DragonInputManager inputHandler;

        public Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Ground and Air Detection Stats")]
        [SerializeField] private float groundDetectionRayStartPoint = 1f;
        [SerializeField] private float minimumDistanceNeededToBeginFall = 2f;
        [SerializeField] private float groundDirectionRayDistance = 0.25f;
        public float inAirTimer;
        public LayerMask ignoreForGroundCheck;

        [Header("Movement Stats")]
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float walkingSpeed = 1f;
        [SerializeField] float rotationSpeed = 10f;
        [SerializeField] float fallingSpeed = 25f;
        [SerializeField] float leapingVelocity = 2.5f;

        // ToDo: maybe local for Falling
        private Vector3 normalVector;
        private Vector3 targetPosition;

        [Header("Stamina Costs")]
        public float rollStaminaCost = 10;
        public float backStepStaminaCost = 5;
        public float sprintStaminaCost = .1f;


        private void Awake()
        {
            cameraHandler = FindObjectOfType<DragonCamera>();

            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<DragonInputManager>();
            playerManager = GetComponent<DragonManager>();

            playerAnimatorManager = GetComponentInChildren<DragonAnimatorManager>();
        }

        private void Start()
        {
            if ( Camera.main != null ) cameraObject = Camera.main.transform;
            else Debug.LogWarning("No Main Camera in the Scene!");

            myTransform = transform;

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }



        public void HandleRotation(float deltaTime)
        {
            Vector3 targetDir = Vector3.zero;
            float movementOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.verticalMovementInput;
            targetDir += cameraObject.right * inputHandler.horizontalMovementInput;

            targetDir.Normalize();
            targetDir.y = 0; // no movement on y-Axis (!)

            if ( targetDir == Vector3.zero )
                targetDir = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * deltaTime);

            myTransform.rotation = targetRotation;

        }

        public void HandleMovement()
        {

            moveDirection = cameraObject.forward * inputHandler.verticalMovementInput;
            moveDirection += cameraObject.right * inputHandler.horizontalMovementInput;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if ( inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f )
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
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

            playerAnimatorManager.HandleAnimatorValues(0, inputHandler.moveAmount, playerManager.isSprinting);
            HandleFalling(Time.deltaTime, moveDirection);
        }


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
                Debug.LogWarning("Player in Air" + inAirTimer);
                inAirTimer++;
                rigidbody.AddForce(transform.forward * leapingVelocity, ForceMode.Impulse);
                rigidbody.AddForce(Vector3.down * fallingSpeed * 9.8f * inAirTimer * deltaTime, ForceMode.Acceleration);
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
                    if ( playerManager.isUsingRootMotion == false )
                    {
                        Debug.LogWarning("Falling");
                        playerAnimatorManager.PlayTargetAnimation("[Airborne] Falling", true);
                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
            }

            // ToDo: put in update ! because it makes the character jittery in fixed update -> Transform manipulation vs Rigidbody
            // maybe calculate velocity required to put the player up

            if ( playerManager.isUsingRootMotion || inputHandler.moveAmount > 0 )
            {
                // rigidbody.MovePosition(targetPosition);
                Debug.LogWarning("Align Feet");
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, deltaTime);
            }
            else
            {
                Debug.LogWarning("Align Feet no Movement");
                // rigidbody.MovePosition(targetPosition);
                myTransform.position = targetPosition;
            }

        }


        public void HandleJumping()
        {
            if ( inputHandler.jumpInput )
            {
                inputHandler.jumpInput = false;
                playerAnimatorManager.PlayTargetAnimation("Standing Jump", true);
                Vector3 forceUp = Vector3.up * 100;
                rigidbody.AddForce(forceUp, ForceMode.Impulse);
                if ( inputHandler.moveAmount > 0 )
                {
                    moveDirection = cameraObject.forward * inputHandler.verticalMovementInput;
                    moveDirection += cameraObject.right * inputHandler.horizontalMovementInput;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;
                }
                // Todo: maye add some force
            }
        }

    }
}