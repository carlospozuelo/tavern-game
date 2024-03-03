using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactuable : MonoBehaviour, IFurniture
{

    protected virtual void OnEnable()
    {
        // TODO: Only add the interactuable if it's inside the tavern area (and not in the house)
        if (CanBeUsedByNPCS())
        {
            NPCController.AddInteractuableForNPC(this);
        }
    }

    protected virtual void OnDisable()
    {
        // TODO: Only add the interactuable if it's inside the tavern area (and not in the house)
        if (CanBeUsedByNPCS())
        {
            NPCController.RemoveInteractuableForNPC(this);
        }
    }


    public abstract void Interact(CharacterAbstract character);

    public abstract bool CanBeUsedByNPCS();

    public abstract float GetMaxDistance();

    public abstract Vector3 GetPosition();

    public abstract GameObject GetGameObject();

    public abstract bool IsInsideObject(Vector3 worldPosition);
    public abstract bool IsPartiallyInsideObject(Vector3 worldPosition);
}
