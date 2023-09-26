using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFurniture
{
    public bool IsInsideObject(Vector3 worldPosition);

    public bool IsPartiallyInsideObject(Vector3 worldPosition);

}
