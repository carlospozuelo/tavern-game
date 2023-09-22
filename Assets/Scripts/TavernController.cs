using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class TavernController : MonoBehaviour
{

    [Serializable]
    public class TavernData
    {
        private List<FurnitureData> furnitures;
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

    }

    [SerializeField]
    private GameObject[] allFurniture, allTaverns, allHouses;

    private Dictionary<string, GameObject> dictionary;
    private Dictionary<string, GameObject> tavernDictionary;

    private static TavernController instance;

    public List<GameObject> placedFurnitures;
    public List<GameObject> currentTaverns;

    [SerializeField]
    private List<GameObject> currentInteractuables;

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

        dictionary = InitializeDictionary(allFurniture);
        tavernDictionary = InitializeDictionary(allTaverns);


        placedFurnitures = new List<GameObject>();
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
            GameObject tavern = Instantiate(tav, GridManager.instance.gameObject.transform);
            list.Add(tavern);
        }

        GridManager.InitializeTilemap();

        return list;
    }

    private Dictionary<string, GameObject> InitializeDictionary(GameObject[] list)
    {
        Dictionary<string, GameObject> d = new Dictionary<string, GameObject>();
        if (list != null)
        {
            foreach (GameObject g in list)
            {
                d.Add(g.name, g);
            }
        }

        return d;
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
        if (f.TryGetComponent(out Interactuable i))
        {
            instance.currentInteractuables.Remove(f);
        }
    }

    public static List<GameObject> GetPlacedFurnitures() { return instance.placedFurnitures; }

    public void SerializeTavern()
    {
        TavernData data = new TavernData();

        foreach (GameObject g in placedFurnitures)
        {
            FurnitureData furniture = g.GetComponent<Furniture>().GetFurnitureData();
            data.AddFurniture(furniture);
        }

        foreach (GameObject g in currentTaverns)
        {
            data.AddTavern(g.name.Replace("(Clone)", ""));
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetPath();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    public void DeSerializeTavern()
    {
        
        foreach (GameObject g in placedFurnitures)
        {
            Destroy(g);
        }
        placedFurnitures = new List<GameObject>();

        TavernData tavern = ReadTavernData();
        if (tavern != null)
        {
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
                foreach (Transform t in GridManager.instance.transform)
                {
                    t.gameObject.SetActive(false);
                }
                foreach (string s in taverns)
                {
                    GameObject tav = Instantiate(instance.tavernDictionary[s], GridManager.instance.gameObject.transform);
                    currentTaverns.Add(tav);
                }
                GridManager.InitializeTilemap();
            }
        }
    }

    public void DeleteData()
    {
        if (File.Exists(GetPath()))
        {
            File.Delete(GetPath());
        }
    }

    public static void InstantiateFurniture(GameObject g, Vector3 worldPosition)
    {
        Vector2 pos = GridManager.instance.SnapPosition(worldPosition);
        GameObject newInstance = Instantiate(g, new Vector3(pos.x, pos.y, g.transform.position.z), Quaternion.identity, null);
        Furniture f = newInstance.GetComponent<Furniture>();
        f.originalPrefab = g;

        UpdateItemsOnTop(pos, newInstance, f);

        AddFurniture(newInstance);
        if (newInstance.TryGetComponent(out Interactuable i))
        {
            instance.currentInteractuables.Add(newInstance);
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
        string path = GetPath();
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            TavernData data = formatter.Deserialize(stream) as TavernData;

            stream.Close();
            return data;
        }
        Debug.Log("No file found on " + GetPath());
        return null;
        
    }

    private string GetPath()
    {
        return Application.persistentDataPath + "/tavern.tav";
    }
}
