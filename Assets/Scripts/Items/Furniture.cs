using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Furniture : MonoBehaviour, Item
{
    [Tooltip("Optional field. If not empty, the furniture can be rotated before placing, and it will be transformed to the referenced gameobject instead")]
    public GameObject rotateGameObject;


    public Vector2Int size = new Vector2Int(1,1);

    public GameObject originalPrefab;

    public FurnitureData GetFurnitureData()
    {
        return new FurnitureData(transform.position - new Vector3(0, 1), originalPrefab.name);
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public bool canBePlacedInside = true;
    public bool canBePlacedOutside = true;
    public bool placedOnWalls = false;
    [Tooltip("Used for rugs or flooring in general. If set to true, the furniture can be placed below items and below the player as well.")]
    public bool rugLike = false;
    public bool canBePlacedBelowItems = false;

    public void UseItem()
    {
        // Place the item on the grid, using the mouse position.
        // Placeholder
        Vector3 worldPosition = GameController.instance.WorldPosition(Input.mousePosition);
        if (CanBePlaced(GridManager.instance.GridPosition(worldPosition)))
        {
            TavernController.InstantiateFurniture(gameObject, worldPosition);
            

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
        if (GameController.instance.DistanceToPlayer(topLeftTile + new Vector3(size.x, -size.y) / 2) >= GameController.instance.maxDistanceToPlaceItems) { return false;  }

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

    public bool IsInsideObject(Vector3 worldPosition)
    {
        worldPosition = GridManager.instance.GridPosition(worldPosition);

        float maxX = transform.position.x + size.x;
        float maxY = transform.position.y - size.y;

        return worldPosition.x >= transform.position.x && worldPosition.x <= maxX
            && worldPosition.y <= transform.position.y && worldPosition.y >= maxY;
    }

    public void PickUp()
    {
        if (PlayerInventory.instance.GetCurrentItem() == null)
        {
            if (GameController.instance.DistanceToPlayer(transform.position + new Vector3(size.x, -size.y) / 2) < GameController.instance.maxDistanceToPlaceItems) {
                PlayerInventory.instance.SetCurrentItem(originalPrefab);
                TavernController.RemoveFurniture(gameObject);
                Destroy(gameObject);
            }
        }
    }

}
