using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LoadFurniture : MonoBehaviour
{

    [MenuItem("Tools/Load furniture")]
    static void Load()
    {
        GameObject[] prefabs = Resources.LoadAll("Furniture", typeof(GameObject)).Cast<GameObject>().ToArray();

        TavernController tavernController = FindObjectOfType<TavernController>();

        tavernController.SetAllFurniture(prefabs);

        /*
        foreach (GameObject g in prefabs)
        {
            tavernController.
        }
        */

    }
}
