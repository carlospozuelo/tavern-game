using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAgent : MonoBehaviour
{
    [SerializeField]
    private string location;

    private Pathfinding pathfinding;

    [SerializeField]
    private int width, height;

    [SerializeField]
    private Vector2 startingPoint;

    [SerializeField]
    private float cellSize = 1f;

    [SerializeField]
    private bool debug = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (debug)
        {
            Gizmos.DrawWireCube(startingPoint  + (new Vector2(width, height) / 4), new Vector3(width, height) / 2);
        }
    }

    public void RecalculateBoundaries()
    {
        pathfinding.UpdateNonWalkableTiles(location);
    }

    public Pathfinding GetPathfinding()
    {
        return pathfinding;
    }

    // Start is called before the first frame update
    void Start()
    {
        LocationController.AddPathfindingAgent(location, this);
        pathfinding = new Pathfinding(width, height, cellSize, startingPoint, debug);

        // Load all obstacles
        pathfinding.UpdateNonWalkableTiles(location);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
