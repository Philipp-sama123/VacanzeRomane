using UnityEngine;

namespace _Scripts {
    public class ResetAnimatorBool : StateMachineBehaviour {
        public string isInteractingBool = "IsInteracting";
        public bool isInteractingStatus = false;

        public string isFiringSpellBool = "IsFiringSpell";
        public bool isFiringSpellStatus = false;
        
        public string canRotateBool = "CanRotate";
        public bool canRotateStatus = true;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(isInteractingBool, isInteractingStatus);
            animator.SetBool(isFiringSpellBool, isFiringSpellStatus);
            animator.SetBool(canRotateBool, canRotateStatus);
        }
    }
}