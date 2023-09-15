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

        public TavernData()
        {
            furnitures = new List<FurnitureData>();
        }

        public void AddFurniture(FurnitureData furniture)
        {
            furnitures.Add(furniture);
        }

        public List<FurnitureData> GetFurniture()
        {
            return furnitures;
        }
    }

    [SerializeField]
    private GameObject[] allFurniture, allTaverns, allHouses;

    private Dictionary<string, GameObject> dictionary;
    private Dictionary<string, GameObject> tavernDictionary;
    private Dictionary<string, GameObject> housesDictionary;

    private static TavernController instance;

    public List<GameObject> placedFurnitures;
    public List<GameObject> currentTaverns;
    public List<GameObject> currentHouses;

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
        housesDictionary = InitializeDictionary(allHouses);


        placedFurnitures = new List<GameObject>();
    }

    public static void UpgradeTavern(GameObject newTavern, bool deleteAll = true)
    {
        instance.currentTaverns = Upgrade(newTavern, deleteAll, instance.currentTaverns);
    }

    private static List<GameObject> Upgrade(GameObject newTavern, bool deleteAll, List<GameObject> list)
    {
        if (deleteAll)
        {
            GridManager.InitializeTilemap();
            foreach (GameObject g in list)
            {
                Destroy(g);
            }
            list = new List<GameObject>();
        }
        GameObject tavern = Instantiate(newTavern, GridManager.instance.gameObject.transform);
        list.Add(tavern);
        GridManager.InitializeTilemap(false);

        return list;
    }

    public static void UpgradeHouse(GameObject newHouse, bool deleteAll = true)
    {
        instance.currentHouses = Upgrade(newHouse, deleteAll, instance.currentHouses);
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
        GameObject instance = Instantiate(g, GridManager.instance.SnapPosition(worldPosition), Quaternion.identity, null);
        instance.GetComponent<Furniture>().originalPrefab = g;
        AddFurniture(instance);

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
