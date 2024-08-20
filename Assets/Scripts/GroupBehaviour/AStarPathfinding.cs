using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinding
{
    public static List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos, Grid grid)
    {
        Vector2Int startGridPos = grid.WorldToGridPosition(startPos);
        Vector2Int targetGridPos = grid.WorldToGridPosition(targetPos);

        if (!grid.IsWithinBounds(startGridPos.x, startGridPos.y))
        {
            Debug.LogError($"Start position ({startPos}) converts to grid position ({startGridPos}) which is out of bounds. Grid size is {grid.Width}x{grid.Height}");
            return null;
        }

        if (!grid.IsWithinBounds(targetGridPos.x, targetGridPos.y))
        {
            Debug.LogError($"Target position ({targetPos}) converts to grid position ({targetGridPos}) which is out of bounds. Grid size is {grid.Width}x{grid.Height}");
            return null;
        }

        Node startNode = grid.Nodes[startGridPos.x, startGridPos.y];
        Node targetNode = grid.Nodes[targetGridPos.x, targetGridPos.y];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(grid, startNode, targetNode);
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.Walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newMovementCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, targetNode);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        Debug.LogWarning("No path found");
        return null;
    }

    static List<Vector2> RetracePath(Grid grid, Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            Vector2 worldPosition = grid.WorldBottomLeft + new Vector2(currentNode.Position.x * grid.NodeSize, currentNode.Position.y * grid.NodeSize);
            path.Add(worldPosition);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }

    static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        int dstY = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
