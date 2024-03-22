using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LoadFurniture : MonoBehaviour
{

    [MenuItem("Tools/Load/Furniture")]
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

    [MenuItem("Tools/Load/Items")]
    static void LoadItems()
    {
        GameObject[] prefabs = Resources.LoadAll("Items", typeof(GameObject)).Cast<GameObject>().ToArray();

        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();

        playerInventory.SetAllItems(prefabs);
    }

    [MenuItem("Tools/Load/Recipes")]
    static void LoadRecipes()
    {
        CraftingRecipe[] recipes = Resources.LoadAll("Recipes", typeof(CraftingRecipe)).Cast<CraftingRecipe>().ToArray();

        CraftingController controller = FindObjectOfType<CraftingController>();

        controller.SetAllRecipes(recipes);
    }
}
