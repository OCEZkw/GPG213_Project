using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    public float moveThreshold = 0.01f; // Minimum movement to be considered walking
    public float idleThreshold = 0.001f; // Maximum movement to be considered idle

    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsIdleHash = Animator.StringToHash("IsIdle");

    private bool isWalking = false;
    private bool isIdle = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on Character!");
        }
        lastPosition = transform.position;
    }

    void Update()
    {
        UpdateMovementState();
        UpdateAnimation();
    }

    void UpdateMovementState()
    {
        Vector3 movement = transform.position - lastPosition;
        float movementMagnitude = movement.magnitude;

        if (movementMagnitude > moveThreshold)
        {
            isWalking = true;
            isIdle = false;
        }
        else if (movementMagnitude < idleThreshold)
        {
            isWalking = false;
            isIdle = true;
        }
        // If movement is between idleThreshold and moveThreshold, we keep the previous state

        lastPosition = transform.position;
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(IsWalkingHash, isWalking);
            animator.SetBool(IsIdleHash, isIdle);
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public bool IsIdle()
    {
        return isIdle;
    }
}