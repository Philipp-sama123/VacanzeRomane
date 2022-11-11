using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts._Dragon;
using UnityEngine;

public class DragonAnimatorManager : MonoBehaviour {
    Animator animator;
    float snappedHorizontal;
    float snappedVertical;
    private DragonManager playerManager;
    private DragonLocomotion playerLocomotion;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerManager = GetComponent<DragonManager>();
        playerLocomotion = GetComponent<DragonLocomotion>();
    }

    public void HandleAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        if ( isSprinting == false )
        {
            animator.SetFloat("Horizontal", SnapValue(horizontalMovement), 0.1f, Time.deltaTime);
            animator.SetFloat("Vertical", SnapValue(verticalMovement), 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Horizontal", SnapValue(horizontalMovement) * 2, 0.1f, Time.deltaTime);
            animator.SetFloat("Vertical", SnapValue(verticalMovement) * 2, 0.1f, Time.deltaTime);
        }
    }


    public void PlayTargetAnimation(string targetAnimation, bool isUsingRootMotion, bool canRotate = false)
    {
        animator.applyRootMotion = isUsingRootMotion;
        animator.SetBool("CanRotate", canRotate); // maybe remove 
        animator.SetBool("IsInteracting", isUsingRootMotion);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    private void OnAnimatorMove()
    {
        if ( playerManager.isUsingRootMotion == false )
            return;

        float delta = Time.deltaTime;
        playerLocomotion.rigidbody.drag = 0;
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        playerLocomotion.rigidbody.velocity = velocity;
    }

    private static float SnapValue(float valueToSnap)
    {
        float snappedValue = 0;

        switch ( valueToSnap )
        {
            case > 0 and < 0.30f :
                snappedValue = 0.25f;
                break;
            case > 0.30f and < 0.55f :
                snappedValue = 0.5f;
                break;
            case > 0.55f and < 0.80f :
                snappedValue = 0.75f;
                break;
            case > 0.80f :
                snappedValue = 1f;
                break;
            case < 0 and > -0.30f :
                snappedValue = -0.25f;
                break;
            case < -0.30f and > -0.55f :
                snappedValue = -0.5f;
                break;
            case < -0.55f and > -0.80f :
                snappedValue = -0.75f;
                break;
            case < -0.80f :
                snappedValue = -1f;
                break;
            default :
                snappedValue = 0;
                break;
        }
        return snappedValue;
    }

}