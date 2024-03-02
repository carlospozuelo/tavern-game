using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private CustomGrid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public Pathfinding(int width, int height, float cellSize, Vector3 originPosition)
    {
        grid = new CustomGrid<PathNode>(width, height, cellSize, originPosition, (CustomGrid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }


    public List<PathNode> GetPathToClosestReachableTile(Vector3 from, Vector3 to, int deepness = 5)
    {
        GetGrid().GetXY(from, out int fromX, out int fromY);
        GetGrid().GetXY(to, out int toX, out int toY);

        return GetPathToClosestReachableTile(fromX, fromY, toX, toY, deepness);

    }

    // Returns the closest tile to xTo yTo, that is reachable from the xFrom yFrom position
    private List<PathNode> GetPathToClosestReachableTile(int fromX, int fromY, int toX, int toY, int deepness = 5)
    {
        for (int total = 0; total < deepness + deepness - 1; total++)
        {
            for (int x = 0; x <= System.Math.Min(total, deepness - 1); x++)
            {
                int y = total - x;
                List<PathNode> list = FindPath(fromX, fromY, toX + x, toY + y);
                if (list != null)
                {
                    return list;
                }
            }
        }

        return null;
    }

    public void UpdateNonWalkableTiles(string location)
    {
        // For each tile, box cast and get ALL the colliders. If at least one of them is in the same location, mark the tile as non-walkable

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Vector2 direction = Vector2.up;
                Vector2 bottomLeftCorner = grid.GetWorldPosition(x,y);
                Vector2 size = new Vector2(grid.GetCellSize(), grid.GetCellSize());

                grid.GetValue(x, y).isWalkable = true;
                grid.UpdateValueText(x, y, Color.white);

                RaycastHit2D[] rays = Physics2D.BoxCastAll(bottomLeftCorner, size, 0f, direction, 0f);
                
                foreach (var ray in rays)
                {
                    if (!ray.collider.CompareTag("Player") && !ray.collider.CompareTag("NPC"))
                    {
                        if (!ray.collider.isTrigger)
                        {
                            // Found at least one collider. Mark the walkable tile as non-walkable
                            grid.GetValue(x, y).isWalkable = false;
                            grid.UpdateValueText(x, y, Color.red);
                            break;
                        }
                    }
                }
            }
        } 

    }

    public CustomGrid<PathNode> GetGrid() { return grid; }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetValue(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                // Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable) { 
                    closedList.Add(neighbourNode);
                    continue; 
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        return null;
    }

    private PathNode GetNode(int x, int y)
    {
        return grid.GetValue(x, y);
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            // Left Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1;  i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;

    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();

        path.Add(endNode);
        PathNode currentNode = endNode;

        while (currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();

        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);

        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }
}
