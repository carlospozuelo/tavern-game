using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is meant to be a controller to store different types of crafting menus and recipes
public class CraftingController : MonoBehaviour
{
    public enum Tables { Basic_Beer_Tap }

    [SerializeField]
    private CraftingRecipe[] recipes;

    public void SetAllRecipes(CraftingRecipe[] recipes) { this.recipes = recipes; }

    private Dictionary<Tables, List<CraftingRecipe>> dictionary;

    private static CraftingController instance;

    private bool initialized = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
            Initialize();
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized) { return; }

        dictionary = new Dictionary<Tables, List<CraftingRecipe>>();

        foreach (Tables type in System.Enum.GetValues(typeof (Tables)))
        {
            dictionary.Add(type, new List<CraftingRecipe>());
        }

        foreach (CraftingRecipe recipe in recipes)
        {
            dictionary[recipe.requiredTable].Add(recipe);
        }

        initialized = true;
    }
}
