using UnityEngine;

namespace _Scripts._Dragon
{
    public class DragonLocomotion : MonoBehaviour
    {
        private DragonManager dragonManager;
        private DragonAnimatorManager dragonAnimatorManager;
        private DragonInputManager dragonInputManager;

        private Vector3 moveDirection;
        private Transform cameraObject;
        public Rigidbody dragonRigidbody;

        [Header("Falling")] public float inAirTimer;
        [SerializeField] private float leapingVelocity = 2.5f;
        [SerializeField] private float fallingVelocity = 35f;
        [SerializeField] private float rayCastHeightOffSet = 0.5f;
        [SerializeField] private float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Movement Speeds")] [SerializeField]
        private float walkingSpeed = 1.5f;

        [SerializeField] private float runningSpeed = 5f;
        [SerializeField] private float sprintingSpeed = 7.5f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Jump Speeds")] [SerializeField]
        private float gravityIntensity = -10f;

        [SerializeField] private float jumpHeight = 3f;

        private void Awake()
        {
            if (Camera.main != null) cameraObject = Camera.main.transform;
            else Debug.LogWarning("[Not Assigned]: Camera");

            dragonManager = GetComponent<DragonManager>();
            dragonInputManager = GetComponent<DragonInputManager>();
            dragonRigidbody = GetComponent<Rigidbody>();

            dragonAnimatorManager = GetComponentInChildren<DragonAnimatorManager>();
        }

        public void HandleAllMovement()
        {
            if (dragonInputManager != null && dragonRigidbody != null)
            {
                HandleFalling();
                HandleRotation();

                if (dragonManager.isInteracting) return;
                if (dragonManager.isJumping) return;

                HandleMovement();
            }
            else Debug.LogWarning("[Not Assigned]: inputManager or playerRigidbody ");
        }

        private void HandleMovement()
        {
            moveDirection = cameraObject.forward * dragonInputManager.verticalInput;
            // -- What? - allows to move left and right
            // -based on camera Object and horizontalInput
            moveDirection += (cameraObject.right * dragonInputManager.horizontalInput);
            moveDirection.Normalize(); // changes Vector Length to one
            moveDirection.y = 0; // keeps the player on the floor 

            if (dragonManager.isSprinting) // because also animations change
            {
                moveDirection *= sprintingSpeed;
            }
            else
            {
                if (dragonInputManager.moveAmount >= 0.5f)
                {
                    moveDirection *= runningSpeed;
                }
                else
                {
                    moveDirection *= walkingSpeed;
                }
            }

            Vector3 movementVelocity = moveDirection;
            dragonRigidbody.velocity = movementVelocity;
        }

        private void HandleRotation()
        {
            Vector3 targetDirection = Vector3.zero;

            targetDirection = cameraObject.forward * dragonInputManager.verticalInput;
            targetDirection += cameraObject.right * dragonInputManager.horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotation =
                Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = playerRotation;
        }

        private void HandleFalling()
        {
            RaycastHit hit;
            Vector3 rayCastOrigin = transform.position;
            Vector3 targetPosition = transform.position;

            rayCastOrigin.y += rayCastHeightOffSet;

            if (!dragonManager.isGrounded && !dragonManager.isJumping)
            {
                if (!dragonManager.isInteracting)
                {
                    dragonAnimatorManager.PlayTargetAnimation("[Airborne] Falling", true);
                    Debug.Log("Here Landing");
                }

                dragonAnimatorManager.animator.SetBool("IsUsingRootMotion", false);

                inAirTimer += Time.deltaTime;
                dragonRigidbody.AddForce(transform.forward * leapingVelocity);
                dragonRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
            }

            Debug.DrawRay(rayCastOrigin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red);
            if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, minimumDistanceNeededToBeginFall,
                    groundLayer))
            {
                if (!dragonManager.isGrounded && dragonManager.isInteracting)
                {
                    if (inAirTimer > 0.25f)
                    {
                        Debug.Log("Here Landing");
                        dragonAnimatorManager.PlayTargetAnimation("[Airborne] Landing", true);
                    }
                    else
                    {
                        dragonAnimatorManager.PlayTargetAnimation("Empty", false);
                    }
                }

                Vector3 raycastHitPoint = hit.point;
                targetPosition.y = raycastHitPoint.y;

                inAirTimer = 0;
                dragonManager.isGrounded = true;
            }
            else
            {
                dragonManager.isGrounded = false;
            }

            if (dragonManager.isGrounded && !dragonManager.isJumping)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.2f);
            }
        }

        public void HandleJumping()
        {
            if (dragonManager.isGrounded)
            {
                dragonAnimatorManager.animator.SetBool("IsJumping", true);
                dragonAnimatorManager.PlayTargetAnimation("[Airborne] Jumping Root", true);

                float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
                Vector3 playerVelocity = moveDirection;
                playerVelocity.y = jumpingVelocity;
                dragonRigidbody.velocity = playerVelocity;
            }

            // Todo: Running Jump
            // Todo: Standing Jump
        }


        public void HandleDodge()
        {
            if (dragonManager.isInteracting) return;

            if (dragonInputManager.moveAmount > 0)
                dragonAnimatorManager.PlayTargetAnimation("[Airborne] StepSlide Forward", true, true);
            else
                dragonAnimatorManager.PlayTargetAnimation("[Airborne] StepSlide Backward", true, true);

            // Todo: Left and Right
            // Todo: Toggle invulnerable State
        }
    }
}