using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderGuardPatrol : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public Transform[] patrolPoints;
    public float waitTime = 1f;
    public List<GuardPatrol> followers = new List<GuardPatrol>();
    public float passThroughDistance = 1f; // Distance at which leader can pass through followers

    private int currentPointIndex = 0;
    private bool isWaiting = false;
    private float waitCounter = 0f;
    private bool movingRight = true;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;

    public string guardName = "Leader";
    public TMP_FontAsset customFont; // New variable for custom font
    private FloatingName floatingName;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();

        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned to leader guard!");
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
        if (isWaiting)
        {
            Wait();
        }
        else
        {
            MoveTowardsPoint();
        }

        animator.SetBool("IsWalking", !isWaiting);
        HandleCollisions();
    }

    void MoveTowardsPoint()
    {
        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (targetPoint.position.x > transform.position.x && !movingRight)
        {
            Flip();
        }
        else if (targetPoint.position.x < transform.position.x && movingRight)
        {
            Flip();
        }

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            isWaiting = true;
            waitCounter = 0f;
        }
    }

    void Wait()
    {
        waitCounter += Time.deltaTime;
        if (waitCounter >= waitTime)
        {
            isWaiting = false;
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        // Reverse the order of followers when turning around
        followers.Reverse();
    }

    void HandleCollisions()
    {
        if (followers.Count == 0) return;

        GuardPatrol lastFollower = followers[followers.Count - 1];
        float distanceToLast = Vector2.Distance(transform.position, lastFollower.transform.position);

        if (distanceToLast < passThroughDistance)
        {
            // Disable collision between leader and followers
            Physics2D.IgnoreCollision(myCollider, lastFollower.GetComponent<Collider2D>(), true);
        }
        else
        {
            // Re-enable collisions
            foreach (var follower in followers)
            {
                Physics2D.IgnoreCollision(myCollider, follower.GetComponent<Collider2D>(), false);
            }
        }
    }

    public void AddFollower(GuardPatrol follower)
    {
        followers.Add(follower);
        follower.leaderGuard = this.transform;
    }
}
