using UnityEngine;

namespace _Scripts._AI {
    [CreateAssetMenu(menuName = "A.I/Enemy Actions/Attack Action")]
    public class EnemyAttackAction : EnemyAction {
        public bool canCombo;
        public EnemyAttackAction comboAction; 
        
        public int attackScore;
        public float recoveryTime;

        public float maximumAttackAngle = 45f;
        public float minimumAttackAngle = -45f;

        public float minimumDistanceNeededToAttack = 0;
        public float maximumDistanceNeededToAttack = 0;
    }
    
}