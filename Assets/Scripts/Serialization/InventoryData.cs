using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class InventoryString
{
    [JsonProperty]
    private string itemId;

    public InventoryString(string id)
    {
        SetId(id);
    }

    public string GetId() { return itemId; }
    public void SetId(string id) { itemId = id; }

    public override string ToString()
    {
        return "itemId: " + itemId;
    }
}

[Serializable]
public class ClothingString : InventoryString
{
    [JsonProperty]
    private string clothingItemId;
    [JsonProperty]
    private SerializableColor c1, c2, c3;

    public ClothingString(string id, ClothingItem.ThreeColors colors) : base("ClothingItem") {
        clothingItemId = id;

        if (colors != null)
        {
            c1 = new SerializableColor(colors.primary);
            c2 = new SerializableColor(colors.secondary);
            c3 = new SerializableColor(colors.tertiary);
        }
    }

    public string GetClothingItemId() { return clothingItemId; }

    public SerializableColor GetSerializableColor1() { return c1; }
    public Color GetColor1() { return c1.GetColor(); }

    public SerializableColor GetSerializableColor2() { return c2; }
    public Color GetColor2() { return c2.GetColor(); }

    public SerializableColor GetSerializableColor3() { return c3; }
    public Color GetColor3() { return c3.GetColor(); }

    public ClothingItem.ThreeColors GetThreeColors()
    {
        ClothingItem.ThreeColors colors = new ClothingItem.ThreeColors();

        colors.primary = new Color(c1.r, c1.g, c1.b, c1.a);
        colors.secondary = new Color(c2.r, c2.g, c2.b, c2.a);
        colors.tertiary = new Color(c3.r, c3.g, c3.b, c3.a);

        return colors;
    }

    public override string ToString()
    {
        return "clothingItem: " + clothingItemId + ", " +  base.ToString();
    }
}


[Serializable]
public class InventoryData
{
    [JsonProperty]
    private InventoryString[] inventory, hotbar;

    [JsonProperty]
    private int gold;

    public InventoryData()
    {
        inventory = new InventoryString[30];
        hotbar = new InventoryString[10];

        this.gold = 0;
    }

    private void HandleCollection(InventoryString[] collection, GameObject[] source)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] != null)
            {
                GameObject g = source[i];

                if (g.TryGetComponent(out Clothing c))
                {
                    collection[i] = new ClothingString(c.GetName(), c.GetThreeColors());
                } else if (g.TryGetComponent(out Item item))
                {
                    collection[i] = new InventoryString(item.GetName());
                }
                else
                {
                    Debug.LogError("Trying to read an item out of " + g + ", but it was not found. Inventory should only contain Items.");
                }
            }
        }

    }

    public InventoryData(GameObject[] inventory, GameObject[] hotbar, int gold)
    {
        this.inventory = new InventoryString[inventory.Length];
        this.hotbar = new InventoryString[hotbar.Length];
        this.gold = gold;

        HandleCollection(this.inventory, inventory);
        HandleCollection(this.hotbar, hotbar);
    }

    private GameObject[] GetCollection(InventoryString[] collection)
    {
        GameObject[] inventory = new GameObject[collection.Length];

        for (int i = 0; i < inventory.Length; i++)
        {
            if (collection[i] != null && !string.IsNullOrEmpty(collection[i].GetId()))
            {

                if (collection[i].GetId().Equals("ClothingItem"))
                {
                    Debug.Log(collection[i]);
                    ClothingString str = (ClothingString) collection[i];

                    ClothingItem item = ClothingController.GetClothingItem(str.GetClothingItemId());
                    inventory[i] = ClothingController.GenerateClothingObject(item, str.GetThreeColors());
                    Debug.Log(inventory[i]);
                }
                else
                {
                    inventory[i] = PlayerInventory.instance.GetItem(collection[i].GetId());
                }
            }
        }

        return inventory;
    }

    public GameObject[] GetInventory()
    {
        return GetCollection(this.inventory);
    }

    public GameObject[] GetHotbar()
    {
        return GetCollection(this.hotbar);
    }

    public int GetGold() { return gold; }
}

