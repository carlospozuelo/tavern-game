using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Furniture : MonoBehaviour, Item
{
    [Tooltip("Optonally include all the sprites for the object. First element faces front, second element faces right, third element faces back and fourth element faces left.")]
    public Sprite[] sprites;
    public Vector2Int size = new Vector2Int(1,1);

    private void Start()
    {
        //size = (int) (size * transform.localScale.x);
    }

    public Sprite GetSprite()
    {

        if (sprites == null || sprites.Length < 1)
            return GetComponent<SpriteRenderer>().sprite;

        return sprites[0];
    }

    public bool canBePlacedInside = true;
    public bool canBePlacedOutside = true;
    public bool canBeRotated = false;
    public bool placedOnWalls = false;
    [Tooltip("Used for rugs or flooring in general. If set to true, the furniture can be placed below items and below the player as well.")]
    public bool rugLike = false;
    public bool canBePlacedBelowItems = false;

    // Instantiate if prefab, otherwise activate the gameobject and change its position ??
    public void UseItem()
    {
        // Place the item on the grid, using the mouse position.
        // Placeholder
        Vector3 worldPosition = GameController.instance.WorldPosition(Input.mousePosition);
        if (CanBePlaced(GridManager.instance.GridPosition(worldPosition)))
        {
            Instantiate(gameObject, GridManager.instance.SnapPosition(worldPosition), Quaternion.identity, null);
            // Consume item from the inventory
            PlayerInventory.instance.ConsumeItem();
            CancelSelectItem();
        }
    }

    public void SelectItem()
    {
        FurniturePreview.instance.EnablePreview(this, transform.localScale);
    }


    public void CancelSelectItem()
    {
        FurniturePreview.instance.DisablePreview();
    }


    // Returns false if the item would overlap with a non-wall collider
    private bool Boxcast(Vector3Int topLeftTile)
    {
        Vector2 direction = Vector2.up;
        Vector2 origin = new Vector2(topLeftTile.x, topLeftTile.y + 1) + new Vector2(size.x, -size.y) / 2;

        RaycastHit2D[] rays = Physics2D.BoxCastAll(origin, size, 0f, direction, 0f);

        foreach (var ray in rays) { 
            if (!ray.collider.name.Equals("Walls"))
            {
                return false;
            }
        }


        return true;
    }



    public bool CanBePlaced(Vector3Int topLeftTile)
    {
        Boxcast(topLeftTile);
        if (GameController.instance.DistanceToPlayer(topLeftTile) >= GameController.instance.maxDistanceToPlaceItems) { return false;  }

        if (placedOnWalls)
        {
            if (!GridManager.instance.IsEntirelyInATilemap(topLeftTile, size, GridManager.TileMaps.FurnishableWall)) { return false; }
        } else
        {
            if (!GridManager.instance.IsEntirelyInATilemap(topLeftTile, size, GridManager.TileMaps.Floor)) { return false; }
        }

        // Check if it would collide with another object
        if (!rugLike && !Boxcast(topLeftTile)) { return false; } 


        return true;
    }

}
