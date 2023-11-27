using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour
{

    [SerializeField]
    private GameObject[] locations;

    private Dictionary<string, GameObject> dictionary;

    private static LocationController instance;

    // Could be changed via savefile in the future
    public string currentLocation = "Tavern";

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

        instance.dictionary[name].SetActive(true);
        instance.currentLocation = name;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        dictionary = new Dictionary<string, GameObject>();

        foreach (var item in locations)
        {
            dictionary[item.name] = item;
        }

        ChangeLocation(currentLocation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
