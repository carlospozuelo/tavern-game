using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    [JsonProperty]
    public float x, y, z; 
    
    [JsonProperty]
    public string name;

    public Vector3 GetPosition() { return new Vector3(x, y, z); }

    public Position(Vector3 position, string name)
    {
        this.name = name;
        x = position.x;
        y = position.y;
        z = position.z;

    }
}
public class Utils
{
    public static Dictionary<string, GameObject> InitializeDictionary(GameObject[] list)
    {
        Dictionary<string, GameObject> d = new Dictionary<string, GameObject>();
        if (list != null)
        {
            foreach (GameObject g in list)
            {
                d.Add(g.name, g);
            }
        }

        return d;
    }
}
