using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour
{

    [SerializeField]
    private GameObject[] locations;

    private Dictionary<string, GameObject> dictionary;

    private static LocationController instance;

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

    // Start is called before the first frame update
    void Start()
    {
        dictionary = new Dictionary<string, GameObject>();

        foreach (var item in locations)
        {
            dictionary[item.name] = item;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
