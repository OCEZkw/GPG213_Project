    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Animator animator;
    public float moveThreshold = 0.01f; // Minimum movement to be considered walking

    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

    private bool isWalking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on Character!");
        }
    }

    public void UpdateMovement(Vector3 movement)
    {
        isWalking = movement.magnitude > moveThreshold;
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(IsWalkingHash, isWalking);
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}