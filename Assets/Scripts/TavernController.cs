using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class TavernData
{
    [JsonProperty]
    private List<FurnitureData> furnitures;
    [JsonProperty]
    private List<string> taverns;

    public TavernData()
    {
        furnitures = new List<FurnitureData>();
        taverns = new List<string>();
    }

    public void AddFurniture(FurnitureData furniture)
    {
        furnitures.Add(furniture);
    }

    public List<FurnitureData> GetFurniture()
    {
        return furnitures;
    }

    public void AddTavern(string tavern)
    {
        taverns.Add(tavern);
    }

    public List<string> GetTaverns()
    {
        return taverns;
    }

    public override string ToString()
    {
        string s = "{\nfurnitures:\n[";

        for (int i = 0; i < furnitures.Count; i++) 
        {
            s += "\n";
            FurnitureData f = furnitures[i];
            s += f;
            if (i + 1 < furnitures.Count) { s += ","; }
            s += "\n";
        }

        s += "],\ntaverns:\n[";

        for (int i = 0; i < taverns.Count; i++)
        {
            s += "\n{\n" + taverns[i] + "\n}";
            if (i+1  < taverns.Count) { s += ","; }
            s += "\n";
        }

        s += "]\n}";

        return s;
    }

}

public class TavernController : MonoBehaviour
{

    
    [SerializeField]
    private GameObject[] allFurniture, allTaverns, allHouses;

    public void SetAllFurniture(GameObject[] f)
    {
        allFurniture = f;
    }

    public static GameObject[] GetAllFurnitureRaw() { return instance.allFurniture; }

    private Dictionary<string, GameObject> dictionary;
    private Dictionary<string, GameObject> tavernDictionary;

    private static TavernController instance;

    public List<GameObject> placedFurnitures;
    public List<GameObject> currentTaverns;

    [SerializeField]
    private List<GameObject> currentInteractuables;

    [SerializeField]
    private Transform furnitureParent;

    public static List<GameObject> GetCurrentInteractuables() { return instance.currentInteractuables;  }

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

        dictionary = Utils.InitializeDictionary(allFurniture);
        tavernDictionary = Utils.InitializeDictionary(allTaverns);

        if (placedFurnitures == null)
        {
            placedFurnitures = new List<GameObject>();
        }
    }

    public static void UpgradeTavern(List<GameObject> newTaverns, bool deleteAll = true)
    {
        instance.currentTaverns = Upgrade(newTaverns, deleteAll, instance.currentTaverns);

        foreach (GameObject g in instance.placedFurnitures)
        {
            Furniture f = g.GetComponent<Furniture>();
            if (f != null)
            {
                f.ReplaceWallFurniture();
            }
        }

    }

    private static List<GameObject> Upgrade(List<GameObject> newTaverns, bool deleteAll, List<GameObject> list)
    {
        if (deleteAll)
        {
            GridManager.Clear();
            list = new List<GameObject>();
        }
        foreach (GameObject tav in newTaverns) { 
            GameObject tavern = Instantiate(tav, GridManager.instance.taverns);
            list.Add(tavern);
        }

        GridManager.InitializeTilemap();

        return list;
    }

    private void Start()
    {
        DeSerializeTavern();
    }

    public static void AddFurniture(GameObject f)
    {
        instance.placedFurnitures.Add(f);
    }

    public static void RemoveFurniture(GameObject f) {
        instance.placedFurnitures.Remove(f);
        foreach (Transform t in f.transform)
        {
            if (t.gameObject.TryGetComponent(out Interactuable i))
            {
                instance.currentInteractuables.Remove(t.gameObject);
            }
        }
    }

    public static List<GameObject> GetPlacedFurnitures() { if (IsActive()) return instance.placedFurnitures; return new List<GameObject>(); }

    public static TavernData GetCurrentTavernData()
    {
        TavernData data = new TavernData();

        foreach (GameObject g in instance.placedFurnitures)
        {
            FurnitureData furniture = g.GetComponent<Furniture>().GetFurnitureData();
            data.AddFurniture(furniture);
        }

        foreach (GameObject g in instance.currentTaverns)
        {
            data.AddTavern(g.name.Replace("(Clone)", ""));
        }

        return data;
    }


    public void DeSerializeTavern()
    {
        TavernData tavern = ReadTavernData();
        if (tavern != null)
        {
            foreach (GameObject g in placedFurnitures)
            {
                Destroy(g);
            }
            placedFurnitures = new List<GameObject>();

            foreach (FurnitureData furniture in tavern.GetFurniture())
            {
                InstantiateFurniture(dictionary[furniture.GetFurnitureName()], furniture.GetPosition());
            }

            foreach (GameObject furniture in placedFurnitures)
            {
                UpdateItemsOnTop(furniture.transform.position, furniture, furniture.GetComponent<Furniture>());
            }

            List<string> taverns = tavern.GetTaverns();
            if (taverns.Count > 0)
            {
                GridManager.Clear();
                currentTaverns = new List<GameObject>();
                foreach (Transform t in GridManager.instance.taverns)
                {
                    t.gameObject.SetActive(false);
                }
                foreach (string s in taverns)
                {
                    GameObject tav = Instantiate(instance.tavernDictionary[s], GridManager.instance.taverns);
                    currentTaverns.Add(tav);
                }
                GridManager.InitializeTilemap();
            }
        } else
        {
            Debug.LogWarning("No tavern data found");
        }
    }

    public void DeleteData()
    {
        if (File.Exists(GetPath()))
        {
            File.Delete(GetPath());
        }
    }

    public static bool IsActive() { return instance.isActiveAndEnabled; }

    public static void InstantiateFurniture(GameObject g, Vector3 worldPosition)
    {
        Vector2 pos = GridManager.instance.SnapPosition(worldPosition);
        GameObject newInstance = Instantiate(g, new Vector3(pos.x, pos.y, g.transform.position.z), Quaternion.identity, instance.furnitureParent);
        Furniture f = newInstance.GetComponent<Furniture>();
        f.originalPrefab = g;

        UpdateItemsOnTop(pos, newInstance, f);

        AddFurniture(newInstance);

        foreach (Transform t in newInstance.transform)
        {
            if (t.TryGetComponent(out Interactuable i))
            {
                instance.currentInteractuables.Add(t.gameObject);
            }
        }
    }

    private static void UpdateItemsOnTop(Vector2 pos, GameObject gO, Furniture f)
    {
        if (f.canBePlacedOnTable)
        {
            foreach (GameObject gf in instance.placedFurnitures)
            {
                if (!gf.Equals(gO))
                {
                    Furniture furniture = gf.GetComponent<Furniture>();
                    if (furniture.IsPartiallyInsideObject(pos) && !furniture.rugLike)
                    {
                        if (!furniture.itemsOnTop.Contains(gO))
                        {
                            furniture.itemsOnTop.Add(gO);
                            f.onTopOf = furniture;
                        }
                    }
                }
            }
        }
    }

    private TavernData ReadTavernData()
    {
        Master data = MasterData.Read();
        if (data != null) { return data.tavernData;  }
        return null;
        
    }

 
    private string GetPath()
    {
        return Application.persistentDataPath + "/tavern.tav";
    }
}
