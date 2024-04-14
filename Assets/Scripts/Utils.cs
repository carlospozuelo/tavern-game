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

    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40,  TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        Color color = Color.white;

        return CreateWorldText(parent, text, localPosition, fontSize, (Color) color, textAnchor, textAlignment, sortingOrder);
    }

    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject go = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = go.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = new Vector3(.1f, .1f, .1f);

        TextMesh textMesh= go.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize * 10;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

        return textMesh;

    }
}
