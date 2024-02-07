using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour
{

    [SerializeField]
    private GameObject[] locations;

    private Dictionary<string, GameObject> dictionary;

    private static LocationController instance;

    [SerializeField]
    private Transform[] allDroppableParents;
    private Dictionary<string, Transform> droppableDictionary;

    // Could be changed via savefile in the future
    public string currentLocation = "Tavern";

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
