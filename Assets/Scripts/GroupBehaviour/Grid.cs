using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Node[,] Nodes;
    public int Width, Height;
    public Vector2 WorldBottomLeft;
    public float NodeSize;

    public Grid(int width, int height, float nodeSize, Vector2 worldBottomLeft)
    {
        Width = width;
        Height = height;
        NodeSize = nodeSize;
        WorldBottomLeft = worldBottomLeft;
        Nodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Nodes[x, y] = new Node(new Vector2Int(x, y), true);
            }
        }
    }

    public Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - WorldBottomLeft.x) / NodeSize);
        int y = Mathf.FloorToInt((worldPosition.y - WorldBottomLeft.y) / NodeSize);
        return new Vector2Int(x, y);
    }


    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.Position.x + x;
                int checkY = node.Position.y + y;

                if (checkX >= 0 && checkX < Width && checkY >= 0 && checkY < Height)
                {
                    neighbors.Add(Nodes[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
}

