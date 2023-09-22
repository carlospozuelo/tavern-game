using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : MonoBehaviour, Interactuable
{
    public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }

    public bool IsInsideObject(Vector3 worldPosition)
    {
        return GetComponent<Furniture>().IsInsideObject(worldPosition);
    }

    public bool IsPartiallyInsideObject(Vector3 worldPosition)
    {
        return GetComponent<Furniture>().IsPartiallyInsideObject(worldPosition);
    }
}
