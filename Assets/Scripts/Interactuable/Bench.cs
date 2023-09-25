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
        PlayerMovement.Sit(transform.position);
        //Debug.Log("Interacting with " + gameObject.name);
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
}
