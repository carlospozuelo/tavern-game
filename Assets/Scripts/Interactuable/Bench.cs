using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : MonoBehaviour, Interactuable
{
    [SerializeField]
    private float maxDistance = 2f;

    [SerializeField]
    private float radius = .5f;

    [SerializeField]
    private Furniture furniture;

    public Transform getUpN, getUpS, getUpE, getUpW;

    public Vector2 direction;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    public float GetMaxDistance()
    {
        return maxDistance;
    }

    public Vector3 GetPosition() {
        return gameObject.transform.position;
    }

    public void Interact()
    {
        if (!PlayerMovement.IsSitting())
        {
            furniture.Block(gameObject);
            PlayerMovement.Sit(transform.position, this);
        }
        
    }

    public bool GetUp(GameObject g, float h, float v)
    {
        if (GetUpPrv(g, h, v))
        {
            furniture.Unblock(gameObject);
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

    public bool IsInsideObject(Vector3 worldPosition)
    {
        //worldPosition = GridManager.instance.GridPosition(worldPosition);

        return Vector2.Distance(worldPosition, transform.position) <= radius;
    }

    public bool IsPartiallyInsideObject(Vector3 worldPosition)
    {
        return IsInsideObject(worldPosition);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
