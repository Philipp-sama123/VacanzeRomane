using _Scripts._AI._States;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts._AI {
    public class EnemyManager : CharacterManager {

        private EnemyLocomotionManager enemyLocomotionManager;
        private EnemyAnimatorManager enemyAnimatorManager;
        private EnemyStats enemyStats;

        public Rigidbody enemyRigidBody;

        public State currentState;
        public CharacterStats currentTarget;
        public NavMeshAgent navmeshAgent;

        public bool isPerformingAction;
        public bool isInteracting;
        public float stoppingDistance = 1.5f;
        public float rotationSpeed = 25f;
        public float maximumAttackRange = 2f;

        [Header("Combat Flags")]
        public bool canDoCombo;

        // public EnemyAttackAction[] enemyAttacks;
        // public EnemyAttackAction currentAttack;
        [Header("A.I. Settings")]
        public float detectionRadius = 20f;
        // the higher, and lower, respectively these angles are the greater detection FIELD OF VIEW (basically like eye sight) 
        public float minimumDetectionAngle = -50f;
        public float maximumDetectionAngle = 50f;
        public float currentRecoveryTime = 0;

        [Header("A.I. Combat Settings")]
        public bool allowAIToPerformCombo = true;
        public float comboLikelyHood = 75; // 100 - SURE attack



        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyStats = GetComponent<EnemyStats>();

            enemyRigidBody = GetComponent<Rigidbody>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            navmeshAgent = GetComponentInChildren<NavMeshAgent>();

            navmeshAgent.enabled = false;
        }

        private void Start()
        {
            enemyRigidBody.isKinematic = false;
        }


        private void Update()
        {
            HandleRecoveryTimer();
            HandleStateMachine();
            isInteracting = enemyAnimatorManager.animator.GetBool("IsInteracting");
            canDoCombo = enemyAnimatorManager.animator.GetBool("CanDoCombo");
            enemyAnimatorManager.animator.SetBool("IsDead", enemyStats.isDead);
        }

        private void FixedUpdate()
        {
            // Reset Navmeshagent
            navmeshAgent.transform.localPosition = Vector3.zero;
            navmeshAgent.transform.localRotation = Quaternion.identity;
        }

        private void HandleStateMachine()
        {
            if ( currentState != null && !enemyStats.isDead )
            {
                State nextState = currentState.Tick(this, enemyStats, enemyAnimatorManager);
                if ( nextState != null )
                {
                    SwitchToNextState(nextState);
                }
            }
        }

        private void SwitchToNextState(State state)
        {
            currentState = state;
        }

        private void HandleRecoveryTimer()
        {
            if ( currentRecoveryTime > 0 )
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            if ( isPerformingAction )
            {
                if ( currentRecoveryTime <= 0 )
                {
                    isPerformingAction = false;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red; //replace red with whatever color you prefer
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}