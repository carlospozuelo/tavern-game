using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private CustomGrid<PathNode> grid;
    public int x, y;

    public int gCost, hCost, fCost;

    public PathNode cameFromNode;
    public bool isWalkable;

    public PathNode(CustomGrid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

}
