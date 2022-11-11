using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts._Dragon;
using UnityEngine;

public class DragonAnimatorManager : MonoBehaviour
{
    Animator animator;
    float snappedHorizontal;
    float snappedVertical;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleAnimatorValues(float horizontalMovement, float verticalMovement)
    {
        if (horizontalMovement > 0)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }

        if (verticalMovement > 0)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }

        animator.SetFloat("Horizontal", snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", snappedVertical, 0.1f, Time.deltaTime);
    }

}