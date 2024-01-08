using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SerializableColor
{
    [JsonProperty]
    public float r, g, b, a;

    public SerializableColor(Color color)
    {
        r = color.r;
        g = color.g;
        b = color.b;

        a = color.a;
    }

    public Color GetColor()
    {
        return new Color(r, g, b, a);
    }
}

[Serializable]
public class ClothingData
{
    [JsonProperty]
    private string itemName;

    // Material Color
    [JsonProperty]
    private SerializableColor color1, color2, color3;

    public ClothingData(string item, Color c1, Color c2, Color c3)
    {
        itemName = item;

        color1 = new SerializableColor(c1);
        color2 = new SerializableColor(c2);
        color3 = new SerializableColor(c3);

    }

    public ClothingItem GetClothingItem()
    {
        ClothingItem[] all = ClothingController.GetAll();

        foreach (ClothingItem item in all)
        {
            if (item.name.Equals(itemName))
            {
                return item;
            }
        }

        Debug.LogWarning("Clothing item not found! Searching for: " + itemName);

        return null;
    }
    
    public Color GetColor1() { return color1.GetColor(); }

    public Color GetColor2() { return color2.GetColor(); }

    public Color GetColor3() { return color3.GetColor(); }

    public override string ToString()
    {
        string s = "{\n";
        s += "itemName: " + itemName + ",\ncolor1: " + GetColor1() + ", color2: " + GetColor2() + ", color3: " + GetColor3() + "\n}";

        return s;
    }

}
