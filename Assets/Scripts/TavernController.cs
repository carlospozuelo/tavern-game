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
    private GameObject[] allFurniture;

    private Dictionary<string, GameObject> dictionary;

    private static TavernController instance;

    public List<GameObject> placedFurnitures;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }

        dictionary = new Dictionary<string, GameObject>();

        foreach(GameObject g in allFurniture)
        {
            dictionary.Add(g.name, g);
        }
        placedFurnitures = new List<GameObject>();
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
