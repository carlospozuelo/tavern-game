using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Master
{
    public TavernData tavernData;

    public PlayerClothingData clothingData;

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
            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                Master master = (Master) serializer.Deserialize(file, typeof(Master));
                return master;
            }
        }
        Debug.Log("No file found on " + GetPath());
        return null;
    }
    

    public static void WriteData()
    {
        instance.data = new Master();

        instance.data.tavernData = TavernController.GetCurrentTavernData();
        instance.data.clothingData = ClothingController.Serialize();

        //string json = JsonUtility.ToJson(this);
        //FileStream stream = new FileStream(GetPath(), FileMode.Create);

        JsonSerializer serializer = new JsonSerializer();

        using (StreamWriter sw = new StreamWriter(GetPath()))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, instance.data);
        }
    }

    private static string GetPath()
    {
        // Modify in the future to support multiple tavern files
        return Application.persistentDataPath + "/tavern.json";
    }
}
