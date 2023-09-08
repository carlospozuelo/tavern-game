using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour, Item
{
    [Tooltip("Optonally include all the sprites for the object. First element faces front, second element faces right, third element faces back and fourth element faces left.")]
    public Sprite[] sprites;
    public int size = 1;

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


    public bool canBePlacedOverItems = false;
    public bool canHaveItemsPlacedOver = false;
    public bool canBePlacedBelowItems = false;

    // Instantiate if prefab, otherwise activate the gameobject and change its position ??
    public void UseItem()
    {
        // Place the item on the grid, using the mouse position.
        // Placeholder
        Instantiate(gameObject, GridManager.instance.SnapPosition(GameController.instance.WorldPosition(Input.mousePosition)), Quaternion.identity, null);
        // Consume item from the inventory
        PlayerInventory.instance.ConsumeItem();
        CancelSelectItem();
    }

    private Coroutine previewCoroutine;

    public void SelectItem()
    {
        FurniturePreview.instance.EnablePreview(this, transform.localScale);
    }


    public void CancelSelectItem()
    {
        FurniturePreview.instance.DisablePreview();
    }

    public bool CanBePlaced(Vector3Int topLeftTile)
    {
        // Check if the item is completely within a wall
        if (placedOnWalls)
        {
            if (!GridManager.instance.IsEntirelyInATilemap(topLeftTile, size, GridManager.TileMaps.FurnishableWall)) { return false; }
        } else
        {
            if (!GridManager.instance.IsEntirelyInATilemap(topLeftTile, size, GridManager.TileMaps.Floor)) { return false; }
        }

        return true;
    }

}
