using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupPatrol : MonoBehaviour
{
    public List<Transform> patrolPoints;
    public List<Character> characters;
    public float moveSpeed = 2f;
    public float arrivalDistance = 0.1f;
    public float cohesionStrength = 0.5f;
    public float separationStrength = 0.5f;
    public float separationRadius = 1f;

    public int gridWidth = 100;
    public int gridHeight = 100;
    public float nodeSize = 1f;

    private int currentPatrolIndex = 0;
    private List<Vector2> currentPath;
    private int currentPathIndex = 0;

    private Grid grid;

    void Start()
    {
        if (patrolPoints.Count == 0)
        {
            Debug.LogError("No patrol points set!");
            return;
        }

        // Calculate the bottom-left corner of the world
        Vector2 worldBottomLeft = CalculateWorldBottomLeft();

        // Initialize the grid
        grid = new Grid(gridWidth, gridHeight, nodeSize, worldBottomLeft);

        // Set some nodes as non-walkable (obstacles)
        // You'll need to customize this based on your game's obstacles
        Vector2Int obstaclePos = grid.WorldToGridPosition(new Vector2(0, 0));
        grid.Nodes[obstaclePos.x, obstaclePos.y].Walkable = false;

        // Get the initial path
        Vector2 startPos = transform.position;
        Vector2 endPos = patrolPoints[currentPatrolIndex].position;

        Debug.Log($"Start position: {startPos}, End position: {endPos}");

        currentPath = AStarPathfinding.FindPath(startPos, endPos, grid);

        if (currentPath == null)
        {
            Debug.LogError("Failed to find initial path. Check start and end positions.");
        }
    }

    private Vector2 CalculateWorldBottomLeft()
    {
        float minX = transform.position.x;
        float minY = transform.position.y;

        foreach (Transform point in patrolPoints)
        {
            if (point.position.x < minX) minX = point.position.x;
            if (point.position.y < minY) minY = point.position.y;
        }

        // Add some padding
        return new Vector2(minX - 10, minY - 10);
    }

    void Update()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            GetNextPath();
            return;
        }

        // Move the group
        Vector2 targetPos = currentPath[currentPathIndex];
        Vector2 moveDirection = (targetPos - (Vector2)transform.position).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Check if we've reached the current path point
        if (Vector2.Distance(transform.position, targetPos) < arrivalDistance)
        {
            currentPathIndex++;
            if (currentPathIndex >= currentPath.Count)
            {
                GetNextPath();
            }
        }

        // Update character positions
        UpdateCharacterPositions();
    }

    void GetNextPath()
    {
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        Vector2 startPos = transform.position;
        Vector2 endPos = patrolPoints[currentPatrolIndex].position;

        Debug.Log($"Getting next path. Start: {startPos}, End: {endPos}");

        currentPath = AStarPathfinding.FindPath(startPos, endPos, grid);

        if (currentPath == null)
        {
            Debug.LogError($"Failed to find path from {startPos} to {endPos}");
        }

        currentPathIndex = 0;
    }

    void UpdateCharacterPositions()
    {
        Vector2 groupCenter = transform.position;

        foreach (Character character in characters)
        {
            Vector2 cohesionForce = (groupCenter - (Vector2)character.transform.position) * cohesionStrength;
            Vector2 separationForce = Vector2.zero;

            foreach (Character otherCharacter in characters)
            {
                if (otherCharacter != character)
                {
                    Vector2 diff = character.transform.position - otherCharacter.transform.position;
                    if (diff.magnitude < separationRadius)
                    {
                        separationForce += diff.normalized / diff.magnitude;
                    }
                }
            }

            separationForce *= separationStrength;

            Vector2 totalForce = cohesionForce + separationForce;
            character.transform.position += (Vector3)totalForce * Time.deltaTime;
        }
    }
}