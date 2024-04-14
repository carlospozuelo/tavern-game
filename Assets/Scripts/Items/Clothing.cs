using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothing : MonoBehaviour, Item
{

    [SerializeField]
    private ClothingItem clothingItem;

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private ClothingItem.ThreeColors colors;

    public ClothingItem.ThreeColors GetThreeColors() { return colors; }

    public void Initialize(ClothingItem item, ClothingItem.ThreeColors colors)
    {
        clothingItem = item;
        this.colors = colors;
    }

    public void Initialize(ClothingItem item, Color c1, Color c2, Color c3)
    {
        ClothingItem.ThreeColors colors = new ClothingItem.ThreeColors(c1, c2, c3);

        Initialize(item, colors);
    }

    public void Wear()
    {
        ClothingController.Wear(clothingItem, colors);
        PlayerInventory.Wear(this);
    }

    public ClothingItem GetClothingItem() { return clothingItem; }

    public void CancelSelectItem()
    {
        // Nothing happens if selected
    }

    public string GetName()
    {
        // Provisional, as clothing items are still provisional and generated procedurally
        return clothingItem.Name;
    }

    public GameObject GetOriginalPrefab()
    {
        return prefab;
    }

    public Sprite GetSprite()
    {
        return clothingItem.sprite;
    }

    public void SelectItem()
    {
        // Nothing happens if selected
    }

    public bool UseItem()
    {
        // Can't be used
        return false;
    }
}
