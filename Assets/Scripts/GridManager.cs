using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public enum TileMaps { Floor, Wall, FurnishableWall }

    public Tilemap floor, wall, furnishableWall;

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
            dictionary = new Dictionary<TileMaps, Tilemap>();
            dictionary[TileMaps.Floor] = floor;
            dictionary[TileMaps.Wall] = wall;
            dictionary[TileMaps.FurnishableWall] = furnishableWall;
        }
    }

    private Dictionary<TileMaps, Tilemap> dictionary;

    public bool IsEntirelyInATilemap(Vector3Int position, Vector2Int size, TileMaps option)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (!dictionary[option].HasTile(position + new Vector3Int(i,-j)))
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

    /*
    private void DebugInfo()
    {
        var worldPos = GameController.instance.WorldPosition(Input.mousePosition);
        var gridPos = GridPosition(worldPos);
        DebugPanelUI.instance.Debug("Mouse world position: " + worldPos
            + "\nGrid position mouse: " + gridPos
            + "\nWall tile on that position? " + IsEntirelyInATilemap(gridPos, 1, TileMaps.FurnishableWall)
            + "\nFloor tile on that position? " + IsEntirelyInATilemap(gridPos, 1, TileMaps.Floor)
            + "\nFloor tile on that postion x4? " + IsEntirelyInATilemap(gridPos, 4, TileMaps.Floor));
    }
    */
}
