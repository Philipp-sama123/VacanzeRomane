using UnityEngine;

namespace _Scripts._Dragon
{
    public class ResetAnimatorBoolDragon : StateMachineBehaviour
    {

        public string isFiringSpellBool = "IsFiringSpell";
        public bool isFiringSpellStatus = false;

        public string useRootMotionBool = "IsUsingRootMotion";
        public bool useRootMotionStatus = false;
        
        public string canRotateBool = "CanRotate";
        public bool canRotateStatus = true;
        

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(isFiringSpellBool, isFiringSpellStatus);
            animator.SetBool(useRootMotionBool, useRootMotionStatus);
            animator.SetBool(canRotateBool, canRotateStatus);
        }
    }
}