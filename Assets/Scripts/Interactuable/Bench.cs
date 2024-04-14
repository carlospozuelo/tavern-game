using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : Interactuable
{
    [SerializeField]
    private float radius = .5f;

    [SerializeField]
    private Furniture furniture;

    public Transform getUpN, getUpS, getUpE, getUpW;

    public Vector2 direction;

    private bool busy = false;

    public bool IsBusy() {  return busy; }

    public Furniture GetFurniture() { return furniture; }

    protected override void OnEnable()
    {
        if (CanBeUsedByNPCS())
        {
            NPCController.AddBenchForNPC(this);
        }
    }

    protected override void OnDisable()
    {
        if (CanBeUsedByNPCS())
        {
            NPCController.RemoveBenchForNPC(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    public override float GetMaxDistance()
    {
        return maxDistance;
    }

    public override Vector3 GetPosition() {
        return gameObject.transform.position;
    }

    public override bool Interact(CharacterAbstract character)
    {
        if (!character.IsSitting() && !busy) //&& !furniture.IsBlocked())
        {
            busy = true;
            furniture.Block(gameObject);
            character.Sit(transform.position, this);
            return true;
        } else
        {
            Debug.LogWarning("Can't interact with this bench. Busy: " + busy + ", character sitting: " + character.IsSitting());
            return false;
        }
        
    }

    public bool GetUp(GameObject g, float h, float v)
    {
        if (GetUpPrv(g, h, v))
        {
            busy = false;
            furniture.Unblock(gameObject);
            NPCController.AddBenchForNPC(this);
            return true;
        }

        return false;
    }

    private bool GetUpPrv(GameObject g, float h, float v)
    {
        float absH = Mathf.Abs(h);
        float absV = Mathf.Abs(v);

        if (absH > absV)
        {
            if (h > 0 && getUpW != null)
            {
                g.transform.position = new Vector3(getUpW.position.x, getUpW.position.y, g.transform.position.z);
                return true;
            }

            if (h < 0 && getUpE != null)
            {
                g.transform.position = new Vector3(getUpE.position.x, getUpE.position.y, g.transform.position.z);
                return true;
            }
        }

        if (v < 0 && getUpS != null)
        {
            g.transform.position = new Vector3(getUpS.position.x, getUpS.position.y, g.transform.position.z);
            return true;
        }

        if (v > 0 && getUpN != null)
        {
            g.transform.position = new Vector3(getUpN.position.x, getUpN.position.y, g.transform.position.z);
            return true;
        }

        return false;
    }

    public override GameObject GetGameObject()
    {
        return gameObject;
    }

    public override bool CanBeUsedByNPCS()
    {
        return true;
    }
}
