using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactuable : MonoBehaviour
{
    [SerializeField]
    protected float maxDistance = 2f;

    public virtual float GetMaxDistance() { return maxDistance; }


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


    public abstract bool Interact(CharacterAbstract character);

    public abstract bool CanBeUsedByNPCS();

    public abstract Vector3 GetPosition();

    public abstract GameObject GetGameObject();
}
