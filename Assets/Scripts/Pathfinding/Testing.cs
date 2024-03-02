using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    Pathfinding pathfinding;
    private void Start()
    {
        print("Creating new pathfinding object");
        pathfinding = new Pathfinding(20,20, 1f, new Vector3(0,0,0));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = GameController.instance.WorldMousePosition();

            pathfinding.GetGrid().GetXY(worldPos, out int x, out int y);

            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
            string str = "";
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f, Color.green, 5f);
                    str += path[i] + ", ";
                }
                str += path[path.Count - 1];
                Debug.Log(str);
            }

}
    }
}
