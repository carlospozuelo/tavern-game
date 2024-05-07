using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    private static Dictionary<string, Portal> portals;

    public string key;
    public string destination;

    public List<string> tags;

    public Vector3 offset;

    public string location = "Tavern";

    public static Portal GetPortal(string name)
    {
        return portals[name];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + offset, 1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (portals == null)
        {
            portals = new Dictionary<string, Portal>();
        }

        if (!portals.ContainsKey(key))
        {
            portals.Add(key, this);
        }
    }

    private void OnDisable()
    {
        portals.Remove(key);
    }

    private void OnDestroy()
    {
        portals.Remove(key);
    }

    private void OnEnable()
    {
        Start();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool validTag = false;
        foreach (string tag in tags) {
            if (collision.CompareTag(tag))
            {
                validTag = true;
                break;
            }        
        }
        if (!validTag) { return; }

        LocationController.ChangeLocation(location);

        Vector3 vector = portals[destination].GetPosition();
        vector.z = collision.transform.position.z;

        collision.transform.position = vector + portals[destination].offset;
    }
}
