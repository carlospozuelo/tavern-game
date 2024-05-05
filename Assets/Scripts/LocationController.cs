using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour
{

    [SerializeField]
    private GameObject[] locations;

    private Dictionary<string, GameObject> dictionary;
    private Dictionary<string, PathfindingAgent> pathfindingAgents;

    private static LocationController instance;

    [SerializeField]
    private Transform[] allDroppableParents;
    private Dictionary<string, Transform> droppableDictionary;

    // Could be changed via savefile in the future
    public string currentLocation = "Tavern";

    public static void AddPathfindingAgent(string location, PathfindingAgent agent)
    {
        if (instance.pathfindingAgents == null)
        {
            instance.pathfindingAgents = new Dictionary<string, PathfindingAgent>();
        }


        if (instance.pathfindingAgents.ContainsKey(location)) { 
            // Replace current agent
            instance.pathfindingAgents.Remove(location);
        }
        instance.pathfindingAgents.Add(location, agent);
    }

    public static PathfindingAgent GetPathfindingAgent(string location)
    {
        if (location == null)
        {
            Debug.LogWarning("Location is null");
            return null;
        }
        if (instance.pathfindingAgents != null && instance.pathfindingAgents.ContainsKey(location))
        {
            return instance.pathfindingAgents[location];
        }

        Debug.LogWarning("Trying to access a pathfinding agent not stored in the controller. (" + location + ")");
        return null;
    }

    public static string GetCurrentLocation() { return instance.currentLocation; }
    public static GameObject GetLocation(string location)
    {
        foreach (GameObject g in instance.locations)
        {
            if (g.name.Equals(location)) return g;
        }

        return null;
    }

    public static Transform GetCurrentLocationDroppable()
    {
        return instance.droppableDictionary[GetCurrentLocation()];
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public static void ChangeLocation(string name)
    {
        if (!instance.initialized)
        {
            instance.Initialize();
        }
        if (!instance.dictionary.ContainsKey(name))
        {
            Debug.LogWarning("The location " + name + " does not exist.");
            return;
        }

        foreach (var keypair in instance.dictionary)
        {
            keypair.Value.SetActive(false);
        }

        if (name.Equals("Tavern"))
        {
            FurniturePreview.instance.EnablePreview();
        } else
        {
            FurniturePreview.instance.HidePreview();
        }

        instance.dictionary[name].SetActive(true);
        instance.currentLocation = name;

        // Select the current item
        PlayerInventory.instance.SelectItem(PlayerInventory.instance.currentItem);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    bool initialized = false;
    void Initialize()
    {
        if (initialized) { return; }
        initialized = true;

        dictionary = new Dictionary<string, GameObject>();

        foreach (var item in locations)
        {
            dictionary[item.name] = item;
        }

        droppableDictionary = new Dictionary<string, Transform>();

        foreach (Transform t in allDroppableParents)
        {
            droppableDictionary[t.parent.name] = t;
        }

        ChangeLocation(currentLocation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
