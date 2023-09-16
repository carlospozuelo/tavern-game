using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Grid grid;

    public static GridManager instance;
    private Dictionary<string, List<Tilemap>> dictionary;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }

    private void Start()
    {
        InitializeTilemap();
    }

    public static void Clear()
    {
        if (instance.dictionary != null && instance.dictionary.ContainsKey("Floor"))
        {
            foreach (Tilemap t in instance.dictionary["Floor"])
            {
                t.transform.parent.gameObject.SetActive(false);//Destroy(t.transform.parent.gameObject);
            }
        }

        instance.dictionary = new Dictionary<string, List<Tilemap>>();
    }

    public static void InitializeTilemap(bool createDic = true)
    {
        if (createDic)
        {
            instance.dictionary = new Dictionary<string, List<Tilemap>>();
        }
        foreach (Transform child in instance.transform)
        {
            if (child.gameObject.activeSelf)
            {
                foreach (Transform granchild in child.transform)
                {
                    // Tilemaps
                    List<Tilemap> tilemaps;
                    if (instance.dictionary.ContainsKey(granchild.name))
                    {
                        tilemaps = instance.dictionary[granchild.name];
                    }
                    else
                    {
                        tilemaps = new List<Tilemap>();
                        instance.dictionary.Add(granchild.name, tilemaps);
                    }
                    Tilemap tilemap = granchild.gameObject.GetComponent<Tilemap>();
                    if (!tilemaps.Contains(tilemap))
                    {
                        tilemaps.Add(tilemap);
                    }
                }
            }
        }
    }



    public bool IsEntirelyInATilemap(Vector3Int position, Vector2Int size, string option)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                // Tilemaps with the same tag
                List<Tilemap> tilemaps = dictionary[option];
                bool found = false;
                foreach (Tilemap t in tilemaps) {
                    if (t.HasTile(position + new Vector3Int(i, -j)))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public Vector3 SnapPosition(Vector3 position)
    {
        return grid.CellToWorld(GridPosition(position)) + new Vector3Int(0,1);
    }

    public Vector3Int GridPosition(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }
    
}
