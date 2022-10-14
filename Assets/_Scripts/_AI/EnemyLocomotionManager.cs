using UnityEngine;

namespace _Scripts._AI {
    public class EnemyLocomotionManager : MonoBehaviour {

        private EnemyManager enemyManager;
        private EnemyAnimatorManager enemyAnimatorManager;
        public Rigidbody enemyRigidbody;

        public CapsuleCollider characterCollider;
        public CapsuleCollider characterCollisionBlockerCollider;
        public LayerMask detectionLayer;

        private void Awake()
        {
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyRigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            enemyRigidbody.isKinematic = false;
            IgnoreCharacterColliderCollision();
        }
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