using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class ClothingData
{
    [JsonProperty]
    private string itemName;

    // Material Color
    [JsonProperty]
    private float color1R, color1G, color1B, color1A;
    [JsonProperty]
    private float color2R, color2G, color2B, color2A;
    [JsonProperty]
    private float color3R, color3G, color3B, color3A;

    public ClothingData(string item, Color c1, Color c2, Color c3)
    {
        itemName = item;

        color1R = c1.r;
        color1G = c1.g;
        color1B = c1.b;
        color1A = c1.a;

        color2R = c2.r;
        color2G = c2.g;
        color2B = c2.b;
        color2A = c2.a;

        color3R = c3.r;
        color3G = c3.g;
        color3B = c3.b;
        color3A = c3.a;

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
    
    public Color GetColor1() { return new Color(color1R, color1G, color1B, color1A); }

    public Color GetColor2() { return new Color(color2R, color2G, color2B, color2A); }

    public Color GetColor3() { return new Color(color3R, color3G, color3B, color3A); }

    public override string ToString()
    {
        string s = "{\n";
        s += "itemName: " + itemName + ",\ncolor1: " + GetColor1() + ", color2: " + GetColor2() + ", color3: " + GetColor3() + "\n}";

        return s;
    }

}
