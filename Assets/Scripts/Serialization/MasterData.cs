using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Master
{
    public string version = "v1";

    public InventoryData inventoryData;

    public TavernData tavernData;

    public PlayerClothingData clothingData;

    public TownData townData;

    public override string ToString()
    {
        return "tavern: " + tavernData + "\nclothing: " + clothingData;
    }
}

public class MasterData : MonoBehaviour
{


    public Master data;

    public static MasterData instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            data = ReadFromFile();

            DontDestroyOnLoad(gameObject);
            if (File.Exists(GetPath())) { SceneManager.LoadScene("Game"); }
        }
    }

    public static Master Read()
    {
        if (instance.data == null)
        {
            instance.data = ReadFromFile();
        }


        return instance.data;
    }

    

    public static Master ReadFromFile()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            Master master = JsonConvert.DeserializeObject<Master>(File.ReadAllText(path), settings);
            return master;
            // deserialize JSON directly from a file
            /*
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                JsonSerializer serializer = new JsonSerializer();

                //string serialized = JsonConvert.SerializeObject(file, settings);
                //Master master = JsonConvert.DeserializeObject<Master>(file, settings);
                
                Master master = (Master) serializer.Deserialize(file, typeof(Master));
                
                return master;
            }
            */
        }
        Debug.Log("No file found on " + GetPath());
        return null;
    }
    

    public static void WriteData()
    {
        instance.data = new Master();

        instance.data.tavernData = TavernController.GetCurrentTavernData();
        instance.data.townData = TownController.GetCurrentTownData();

        instance.data.clothingData = ClothingController.Serialize();

        instance.data.inventoryData = PlayerInventory.instance.Serialize();

        //string json = JsonUtility.ToJson(this);
        //FileStream stream = new FileStream(GetPath(), FileMode.Create);
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        File.WriteAllText(GetPath(), JsonConvert.SerializeObject(instance.data, Formatting.Indented, settings));
        /*
        JsonSerializer serializer = new JsonSerializer();
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        string data = JsonConvert.SerializeObject(instance.data, settings);

        using (StreamWriter sw = new StreamWriter(GetPath()))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, data);//instance.data);
        }
        */
    }

    private static string GetPath()
    {
        // Modify in the future to support multiple tavern files
        return Application.persistentDataPath + "/tavern.json";
    }
}
