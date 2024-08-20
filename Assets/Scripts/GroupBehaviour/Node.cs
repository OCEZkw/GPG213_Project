using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int Position;
    public bool Walkable;
    public int GCost;
    public int HCost;
    public Node Parent;

    public int FCost { get { return GCost + HCost; } }

    public Node(Vector2Int pos, bool walkable)
    {
        Position = pos;
        Walkable = walkable;
    }
}
