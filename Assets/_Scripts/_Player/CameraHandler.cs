using System.Collections.Generic;
using UnityEngine;

namespace _Scripts._Player {
    public class CameraHandler : MonoBehaviour {
        private PlayerManager playerManager;
        private InputHandler inputHandler;
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        public Transform myTransform;

        public Vector3 cameraTransformPosition;

        public LayerMask ignoreLayers;
        public LayerMask environmentLayer;

        private Vector3 cameraFollowVelocity = Vector3.zero;

        private static CameraHandler _singleton;

        public float lookSpeed = 0.05f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;

        public float minimumPivot = -45f;
        public float maximumPivot = 45f;

        public float cameraSphereRadius = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        [Header("Camera Height for LockOn State")]
        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;


        [Header("Lock On Target")]
        public List<CharacterManager> availableTargets;
        public CharacterManager currentLockOnTarget;
        public CharacterManager nearestLockOnTarget;

        public CharacterManager leftLockTarget;
        public CharacterManager rightLockTarget;

        public float maximumLockOnDistance = 30f;

        private void Awake()
        {
            _singleton = this;
            myTransform = transform;

            defaultPosition = cameraTransform.localPosition.z;
            // environmentLayer = ~(1 << 8 | 1 << 9 | 1 << 12);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
            inputHandler = FindObjectOfType<InputHandler>();
            playerManager = FindObjectOfType<PlayerManager>();

            // Camera is to fast without it
            // Application.targetFrameRate = 60;
        }

        private void Start()
        {
            environmentLayer = LayerMask.NameToLayer("Environment");
        }

        public void FollowTarget(float deltaTime)
        {
            Vector3 targetPositionTemp = Vector3.SmoothDamp(myTransform.position, targetTransform.position,
                ref cameraFollowVelocity, deltaTime / followSpeed);
            myTransform.position = targetPositionTemp;

            HandleCameraCollisions(deltaTime);
        }

        public void HandleCameraRotation(float deltaTime, float mouseXInput, float mouseYInput)
        {
            if ( inputHandler.lockOnFlag == false && currentLockOnTarget == null )
            {
                lookAngle += (mouseXInput * lookSpeed) ;
                pivotAngle -= (mouseYInput * pivotSpeed);
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                myTransform.rotation = targetRotation;

                rotation = Vector3.zero;
                rotation.x = pivotAngle;

                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            else
            {
                var position = currentLockOnTarget.transform.position;
                Vector3 dir = position - transform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                dir = position - cameraPivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }
        }

        private void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if ( Physics.SphereCast
                (cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition)
                    , ignoreLayers) )
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis);
            }

            if ( Mathf.Abs(targetPosition) < minimumCollisionOffset )
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }

        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26f);

            for ( int i = 0; i < colliders.Length; i++ )
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                if ( character != null )
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                    RaycastHit hit;

                    if ( character.transform.root != targetTransform.transform.root
                         && viewableAngle > -50 && viewableAngle < 50
                         && distanceFromTarget <= maximumLockOnDistance )
                    {
                        if ( Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.transform.position, out hit) )
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.transform.position);
                            if ( hit.transform.gameObject.layer == environmentLayer )
                            {
                                //cannot lock onto target
                            }
                            else
                            {
                                availableTargets.Add(character);
                            }
                        }
                    }
                }
            }
            for ( int k = 0; k < availableTargets.Count; k++ )
            {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                if ( distanceFromTarget < shortestDistance )
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }

                if ( inputHandler.lockOnFlag )
                {
                    Vector3 relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;


                    if ( relativeEnemyPosition.x < 0f && distanceFromLeftTarget > shortestDistanceOfLeftTarget
                                                      && availableTargets[k] != currentLockOnTarget )
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = availableTargets[k];
                    }
                    else if ( relativeEnemyPosition.x > 0f && distanceFromRightTarget < shortestDistanceOfRightTarget
                                                           && availableTargets[k] != currentLockOnTarget )
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockTarget = availableTargets[k];
                    }
                }
            }
        }

        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            currentLockOnTarget = null;
            nearestLockOnTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

            if ( currentLockOnTarget != null )
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }

        }
    }
}