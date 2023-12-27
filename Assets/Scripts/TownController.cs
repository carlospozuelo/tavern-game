using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TownData
{
    // Town hall, tavern...
    public List<Position> townBuildings;

    // Trees, weeds...
    public List<Position> townItems;

    public TownData()
    {
        townBuildings = new List<Position>();

        townItems = new List<Position>();
    }

    public void AddBuilding(GameObject g, string type)
    {
        townBuildings.Add(new Position(g.transform.position, g.name.Replace("(Clone)", "")));
    }

    public void AddItem(GameObject g)
    {
        townItems.Add(new Position(g.transform.position, g.name.Replace("(Clone)", "")));
    }

}

public class TownController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] allBuildings, allTownItems;

    private Dictionary<string, GameObject> buildings;
    private Dictionary<string, GameObject> townItems;

    [SerializeField]
    private Transform buildingParent, townItemsParent;

    public GameObject tavern, townhall;

    private static TownController instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }

        buildings = Utils.InitializeDictionary(allBuildings);
        townItems = Utils.InitializeDictionary(allTownItems);

        DeserializeTown();
    }


    public static void UpgradeTavern(GameObject newTavernPrefab)
    {
        GameObject g = Instantiate(newTavernPrefab, instance.tavern.transform.position, Quaternion.identity, instance.buildingParent);

        Destroy(instance.tavern);

        instance.tavern = g;
    }

    public static void UpgradeTH(GameObject newTHPrefab)
    {
        GameObject g = Instantiate(newTHPrefab, instance.townhall.transform.position, Quaternion.identity, instance.buildingParent);

        Destroy(instance.townhall);

        instance.townhall = g;
    }

    private void Start()
    {
        
    }

    public static TownData GetCurrentTownData()
    {
        TownData data = new TownData();

        foreach (Transform child in instance.buildingParent)
        {
            data.AddBuilding(child.gameObject, child.gameObject.tag);
        }

        foreach (Transform child in instance.townItemsParent)
        {
            data.AddItem(child.gameObject);
        }

        return data;
    }

    private void DeserializeTown()
    {
        TownData data = ReadData();

        if (data != null && data.townBuildings.Count > 0)
        {
            // Delete default town
            foreach (Transform child in buildingParent) { Destroy(child.gameObject); }
            foreach (Transform child in townItemsParent) { Destroy(child.gameObject); }
            
            // Instantiate new town
            foreach (Position p in data.townItems)
            {
                if (instance.townItems.ContainsKey(p.name))
                {
                    GameObject prefab = instance.townItems[p.name];
                    Instantiate(prefab, p.GetPosition(), Quaternion.identity, townItemsParent);
                } else
                {
                    Debug.LogWarning("Town item with key " +  p.name + " not found in dictionary.");
                }
            }

            foreach (Position p in data.townBuildings)
            {
                if (instance.buildings.ContainsKey(p.name))
                {
                    GameObject prefab = instance.buildings[p.name];
                    GameObject g = Instantiate(prefab, p.GetPosition(), Quaternion.identity, buildingParent);

                    if (p.name.Contains("Tavern"))
                    {
                        instance.tavern = g;
                    }

                    else if (p.name.Contains("Townhall"))
                    {
                        instance.townhall = g;
                    }

                }
                else
                {
                    Debug.LogWarning("Building with key " + p.name + " not found in dictionary.");
                }
            }

        } else
        {
            Debug.LogWarning("No town data found");
        }
    }

    public TownData ReadData()
    {
        Master m = MasterData.Read();

        if (m == null) return null;
        return m.townData;
    }
}
