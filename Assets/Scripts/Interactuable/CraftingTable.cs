using System.Collections.Generic;
using UnityEngine;

// This class represents an interactable furniture that opens a crafting menu
public class CraftingTable : SlottableAbstract
{

    public CraftingController.Tables tableType;

    public float delay = 4f;


    public override bool CanBeUsedByNPCS()
    {
        // Only for player
        return false;
    }

    public override GameObject GetGameObject()
    {
        return gameObject;
    }

    public override Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }



}
