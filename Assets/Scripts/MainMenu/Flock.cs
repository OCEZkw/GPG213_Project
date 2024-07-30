using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehaviour behaviour;

    [Range(10, 500)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighbourRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighbourRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    public Vector2 swimAreaSize = new Vector2(10f, 10f);
    public Vector2 swimAreaCenter = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighbourRadius = neighbourRadius * neighbourRadius;
        squareAvoidanceRadius = squareNeighbourRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
        for (int i = 0; i < startingCount; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(-swimAreaSize.x / 2, swimAreaSize.x / 2),
                Random.Range(-swimAreaSize.y / 2, swimAreaSize.y / 2)
            ) + swimAreaCenter;

            FlockAgent newAgent = Instantiate(
                agentPrefab,
                randomPos,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
            );
            newAgent.name = "Agent " + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);
            Vector2 move = behaviour.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }

            // Keep within swim area
            move = KeepWithinSwimArea(agent.transform.position, move);

            // Move the agent
            agent.Move(move);
        }
    }

    private Vector2 KeepWithinSwimArea(Vector2 currentPosition, Vector2 moveVector)
    {
        Vector2 newPosition = currentPosition + moveVector;

        float leftBound = swimAreaCenter.x - swimAreaSize.x / 2;
        float rightBound = swimAreaCenter.x + swimAreaSize.x / 2;
        float bottomBound = swimAreaCenter.y - swimAreaSize.y / 2;
        float topBound = swimAreaCenter.y + swimAreaSize.y / 2;

        if (newPosition.x < leftBound) moveVector.x = Mathf.Max(0, moveVector.x);
        if (newPosition.x > rightBound) moveVector.x = Mathf.Min(0, moveVector.x);
        if (newPosition.y < bottomBound) moveVector.y = Mathf.Max(0, moveVector.y);
        if (newPosition.y > topBound) moveVector.y = Mathf.Min(0, moveVector.y);

        return moveVector;
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighbourRadius);
        foreach(Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = (Vector3)swimAreaCenter;
        Vector3 size = new Vector3(swimAreaSize.x, swimAreaSize.y, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}
