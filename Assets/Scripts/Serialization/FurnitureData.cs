using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class FurnitureData
{
    [JsonProperty]
    private float x, y, z;
    [JsonProperty]
    private string furnitureName;

    public FurnitureData(Vector3 position, string furnitureName)
    {
        x = position.x;
        y = position.y;
        z = position.z;

        this.furnitureName = furnitureName;
    }

    public Vector3 GetPosition() { return new Vector3(x, y, z); }
    public string GetFurnitureName() {  return furnitureName; }

    public override string ToString()
    {
        string s = "{\n";

        s += FurnitureString() + "\n}";

        return s;
    }

    protected string FurnitureString()
    {
        return "x: " + x + ", y:" + y + ", z: " + z + ",\nfurnitureName: " + furnitureName;
    }
}

public class FurnitureWithSlotsData : FurnitureData
{
    [JsonProperty]
    public List<InventoryString> slots;

    public FurnitureWithSlotsData(Vector3 position, string furnitureName) : base(position, furnitureName)
    {
        slots = new List<InventoryString>();
    }
}
