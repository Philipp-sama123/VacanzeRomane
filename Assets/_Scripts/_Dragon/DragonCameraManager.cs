using System.Collections.Generic;
using _Scripts._Player;
using UnityEngine;

namespace _Scripts._Dragon
{
    public class DragonCameraManager : MonoBehaviour
    {
        DragonInputManager dragonInputManager;
        DragonManager dragonManager;

        public Transform targetTransform; //The object the camera will follow
        public Transform cameraPivotTransform; //The object the camera uses to pivot (Look up and down)
        public Transform cameraTransform; //The transform of the actual camera object in the scene

        public LayerMask collisionLayers; //The layers we want our camera to collide with
        public LayerMask environmentLayer;

        private float defaultPosition;
        private Vector3 cameraFollowVelocity = Vector3.zero;
        private Vector3 cameraVectorPosition;

        public float cameraCollisionOffSet = 0.2f; //How much the camera will jump off of objects its colliding with
        public float minimumCollisionOffSet = 0.2f;
        public float cameraCollisionRadius = 2;
        public float cameraFollowSpeed = 0.1f;
        public float cameraLookSpeed = 2;
        public float cameraPivotSpeed = 2;

        public float lookAngle; //Camera looking up and down
        public float pivotAngle; //Camera looking left and right
        public float minimumPivotAngle = -45;
        public float maximumPivotAngle = 45;

        [Header("Camera Height for LockOn State")]
        public float lockedPivotPosition = 2.25f;

        public float unlockedPivotPosition = 1.65f;

        [Header("Lock On Target")] public List<CharacterManager> availableTargets;
        public CharacterManager currentLockOnTarget;
        public CharacterManager nearestLockOnTarget;

        public CharacterManager leftLockTarget;
        public CharacterManager rightLockTarget;

        public float maximumLockOnDistance = 30f;


        private void Awake()
        {
            dragonManager = FindObjectOfType<DragonManager>();
            dragonInputManager = FindObjectOfType<DragonInputManager>();
            targetTransform = dragonManager.lockOnTransform.transform;
            if (Camera.main != null) cameraTransform = Camera.main.transform;
            defaultPosition = cameraTransform.localPosition.z;
            // collisionLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        public void HandleAllCameraMovement(float deltaTime, float cameraInputX, float cameraInputY)
        {
            FollowTarget(deltaTime);
            RotateCamera(deltaTime, cameraInputX, cameraInputY);
        }

        private void FollowTarget(float deltaTime)
        {
            Vector3 targetPosition = Vector3.SmoothDamp
                (transform.position, targetTransform.position, ref cameraFollowVelocity, deltaTime / cameraFollowSpeed);

            transform.position = targetPosition;
            HandleCameraCollisions(deltaTime);
        }

        private void RotateCamera(float deltaTime, float cameraInputX, float cameraInputY)
        {
            if (dragonInputManager.lockOnFlag == false && currentLockOnTarget == null)
            {
                lookAngle += (cameraInputX * cameraLookSpeed);
                pivotAngle -= (cameraInputY * cameraPivotSpeed);
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                transform.rotation = targetRotation;

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

        private void HandleCameraCollisions(float deltaTime)
        {
            float targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast
                (cameraPivotTransform.transform.position, cameraCollisionRadius, direction, out hit,
                    Mathf.Abs(targetPosition), collisionLayers))
            {
                float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(distance - cameraCollisionOffSet);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
            {
                targetPosition = targetPosition - minimumCollisionOffSet;
            }

            cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
            // cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, deltaTime/0.2f);
            cameraTransform.localPosition = cameraVectorPosition;
        }

        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26f);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                if (character != null)
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                    RaycastHit hit;

                    if (character.transform.root != targetTransform.transform.root
                        && viewableAngle > -50 && viewableAngle < 50
                        && distanceFromTarget <= maximumLockOnDistance)
                    {
                        if (Physics.Linecast(dragonManager.lockOnTransform.position,
                                character.lockOnTransform.transform.position, out hit))
                        {
                            Debug.DrawLine(dragonManager.lockOnTransform.position,
                                character.lockOnTransform.transform.position);
                            if (hit.transform.gameObject.layer == environmentLayer)
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

            for (int k = 0; k < availableTargets.Count; k++)
            {
                float distanceFromTarget =
                    Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }

                if (dragonInputManager.lockOnFlag)
                {
                    Vector3 relativeEnemyPosition =
                        dragonInputManager.transform.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;


                    if (relativeEnemyPosition.x < 0f && distanceFromLeftTarget > shortestDistanceOfLeftTarget
                                                     && availableTargets[k] != currentLockOnTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = availableTargets[k];
                    }
                    else if (relativeEnemyPosition.x > 0f && distanceFromRightTarget < shortestDistanceOfRightTarget
                                                          && availableTargets[k] != currentLockOnTarget)
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

            if (currentLockOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                    cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                    cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }
}