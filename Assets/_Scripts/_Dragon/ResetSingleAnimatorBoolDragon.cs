using UnityEngine;

namespace _Scripts._Dragon {
    public class ResetSingleAnimatorBoolDragon : StateMachineBehaviour {
        public string animatorBool = "IsJumping";
        public bool animatorBoolStatus = false;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(animatorBool, animatorBoolStatus);
        }
    }
}