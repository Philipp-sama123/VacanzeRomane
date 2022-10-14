using UnityEngine;

namespace _Scripts {
    public class AnimatorManager : MonoBehaviour {

        public Animator animator;
        public bool canRotate;

        public void PlayTargetAnimation(string targetAnimation, bool isInteractingAnimation, bool canRotate = false)
        {
            animator.applyRootMotion = isInteractingAnimation;
            animator.SetBool("CanRotate", canRotate); // maybe remove 
            animator.SetBool("IsInteracting", isInteractingAnimation);
            animator.CrossFade(targetAnimation, 0.2f);
        }

        public virtual void TakeCriticalDamageAnimationEvent()
        {
            Debug.Log("[Info] critical Damage called!");
        }
    }
}