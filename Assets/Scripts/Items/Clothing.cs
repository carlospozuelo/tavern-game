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
    private SpriteRenderer spriteRenderer;

    void Start()
    {

    }

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

    public void UseItem()
    {
        // Can't be used
    }
}
