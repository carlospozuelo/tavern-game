using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactuable : IFurniture
{

    public void Interact();

    public float GetMaxDistance();

    public Vector3 GetPosition();

    public GameObject GetGameObject();

}
