using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactuable : IFurniture
{

    public void Interact(CharacterAbstract character);

    public float GetMaxDistance();

    public Vector3 GetPosition();

    public GameObject GetGameObject();

}
