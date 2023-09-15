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

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
            dictionary = new Dictionary<string, List<Tilemap>>();

            
        }
    }

    private void Start()
    {
        foreach (Transform child in transform) { 
            foreach (Transform granchild in child.transform)
            {
                // Tilemaps
                List<Tilemap> tilemaps;
                if (dictionary.ContainsKey(granchild.name))
                {
                    tilemaps = dictionary[granchild.name];
                } else
                {
                    tilemaps = new List<Tilemap>();
                    dictionary.Add(granchild.name, tilemaps);
                }
                tilemaps.Add(granchild.gameObject.GetComponent<Tilemap>());
            }
        }

        foreach (var keyValuePair in dictionary)
        {
            Debug.Log("Items in: " + keyValuePair.Key);
            foreach (var tilemap in keyValuePair.Value)
            {
                Debug.Log(tilemap);
            }
        }
    }

    private Dictionary<string, List<Tilemap>> dictionary;

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
