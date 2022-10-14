using UnityEngine;

namespace _Scripts._AI._States {
    public abstract class State : MonoBehaviour {
        public abstract State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager);
    }
}