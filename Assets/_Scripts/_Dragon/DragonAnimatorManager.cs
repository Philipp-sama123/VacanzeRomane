using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts._Dragon;
using UnityEngine;

public class DragonAnimatorManager : MonoBehaviour
{
     public Animator animator;

    private DragonManager playerManager;
    private Rigidbody playerRigidbody;
    
    private int Vertical;
    private int Horizontal;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        playerManager = GetComponentInParent<DragonManager>();
        playerRigidbody = GetComponentInParent<Rigidbody>();

        Horizontal = Animator.StringToHash("Horizontal");
        Vertical = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontal;
        float snappedVertical;
        #region Snapped Horizontal

        if ( horizontalMovement > 0 && horizontalMovement < 0.55f )
        {
            snappedHorizontal = 0.5f;
        }
        else if ( horizontalMovement > 0.55f )
        {
            snappedHorizontal = 1;
        }
        else if ( horizontalMovement < 0 && horizontalMovement > -0.55f )
        {
            snappedHorizontal = -0.5f;
        }
        else if ( horizontalMovement < -0.55f )
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }

        #endregion
        #region Snapped Vertical

        if ( verticalMovement > 0 && verticalMovement < 0.55f )
        {
            snappedVertical = 0.5f;
        }
        else if ( verticalMovement > 0.55f )
        {
            snappedVertical = 1;
        }
        else if ( verticalMovement < 0 && verticalMovement > -0.55f )
        {
            snappedVertical = -0.5f;
        }
        else if ( verticalMovement < -0.55f )
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }

        #endregion

        if ( isSprinting )
        {
            snappedVertical = 2;
            // snappedHorizontal = 2;
        }

        animator.SetFloat(Horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(Vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting, bool useRootMotion = false)
    {
        animator.SetBool("IsInteracting", isInteracting);
        animator.SetBool("IsUsingRootMotion", useRootMotion);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    // checks every Frame Animation is played
    private void OnAnimatorMove()
    {
        if ( playerManager.isUsingRootMotion )
        {
            playerRigidbody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;

            // Todo: change for Animations where you wanna Jump
            deltaPosition.y = 0;

            Vector3 velocity = deltaPosition / Time.deltaTime;
            playerRigidbody.velocity = velocity;

        }
    }
}