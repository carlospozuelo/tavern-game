using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : SlottableAbstract
{
    public override bool CanBeUsedByNPCS()
    {
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
