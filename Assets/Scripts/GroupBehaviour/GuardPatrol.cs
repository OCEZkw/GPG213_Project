using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuardPatrol : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float followDistance = 1.5f;
    public Transform leaderGuard;

    private bool movingRight = true;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;

    public string guardName = "Guard";
    public TMP_FontAsset customFont; // New variable for custom font
    private FloatingName floatingName;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();

        if (leaderGuard == null)
        {
            Debug.LogError("No leader guard assigned!");
        }

        // Set up floating name
        GameObject nameTextObj = new GameObject(guardName + " Text");
        nameTextObj.transform.SetParent(this.transform);
        nameTextObj.transform.localPosition = Vector3.zero;

        TextMeshPro tmpComponent = nameTextObj.AddComponent<TextMeshPro>();
        tmpComponent.alignment = TextAlignmentOptions.Center;
        tmpComponent.fontSize = 3;
        if (customFont != null)
        {
            tmpComponent.font = customFont;
        }

        floatingName = nameTextObj.AddComponent<FloatingName>();
        floatingName.target = this.transform;
        floatingName.guardName = guardName;
    }

    void Update()
    {
        FollowLeader();
        HandleCollisions();
    }

    void FollowLeader()
    {
        if (leaderGuard == null) return;

        Vector2 directionToLeader = leaderGuard.position - transform.position;
        float distanceToLeader = directionToLeader.magnitude;

        bool isMoving = distanceToLeader > followDistance;
        animator.SetBool("IsWalking", isMoving);

        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, leaderGuard.position, moveSpeed * Time.deltaTime);

            if (directionToLeader.x > 0 && !movingRight)
            {
                Flip();
            }
            else if (directionToLeader.x < 0 && movingRight)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void HandleCollisions()
    {
        LeaderGuardPatrol leaderScript = leaderGuard.GetComponent<LeaderGuardPatrol>();
        if (leaderScript != null)
        {
            float distanceToLeader = Vector2.Distance(transform.position, leaderGuard.position);
            if (distanceToLeader < leaderScript.passThroughDistance)
            {
                Physics2D.IgnoreCollision(myCollider, leaderGuard.GetComponent<Collider2D>(), true);
            }
        }
    }
}