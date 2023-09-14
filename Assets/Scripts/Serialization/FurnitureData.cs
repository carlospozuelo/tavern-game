using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FurnitureData
{
    private float x, y, z;
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
}
