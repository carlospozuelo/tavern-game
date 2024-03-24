using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is meant to be a controller to store different types of crafting menus and recipes
public class CraftingController : MonoBehaviour
{
    public enum Tables { Basic_Beer_Tap }

    [SerializeField]
    private CraftingRecipe[] recipes;
    [SerializeField]
    private MenuUI[] allMenus;

    private MenuUI openedMenu;

    public static void SetOpenedMenu(MenuUI menu)
    {
        instance.openedMenu = menu;
    }

    public void SetAllRecipes(CraftingRecipe[] recipes) { this.recipes = recipes; }

    private Dictionary<Tables, List<CraftingRecipe>> dictionary;
    private Dictionary<string, MenuUI> menus;

    private static CraftingController instance;

    private bool initialized = false;

    public static bool anyOpen = false;

    private GameObject[] slots;

    public static GameObject GetOpenIndex(int index)
    {
        if (instance.slots != null)
        {
            return instance.slots[index];
        }

        return null;
    }

    public static void SetOpenIndex(int index, GameObject g)
    {
        if (instance.slots != null)
        {
            if (index < instance.slots.Length)
            {
                instance.slots[index] = g;
            }
        }
    }

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

    public static void UpdateAll()
    {
        foreach (MenuUI menu in instance.allMenus)
        {
            if (menu != null)
            {
                menu.UpdateImage();
}
        }
    }

    public static void CloseAllMenus(float timescale)
    {
        foreach (MenuUI menu in instance.allMenus)
        {
            if (menu != null)
            {
                menu.Close(timescale);
            } else
            {
                Debug.LogError("Menu is null: " + menu);
            }
        }
    }


    public static bool OpenMenu(string name, GameObject[] slots = null)
    {
        if (instance.menus.TryGetValue(name, out MenuUI menu))
        {
            //InventoryUI.
            instance.slots = slots;
            menu.Open();
        }

        return false;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized) { return; }

        dictionary = new Dictionary<Tables, List<CraftingRecipe>>();
        menus = new Dictionary<string, MenuUI>();

        foreach (Tables type in System.Enum.GetValues(typeof (Tables)))
        {
            dictionary.Add(type, new List<CraftingRecipe>());
        }

        foreach (CraftingRecipe recipe in recipes)
        {
            dictionary[recipe.requiredTable].Add(recipe);
        }

        foreach (MenuUI menu in allMenus)
        {
            menus.Add(menu.menu.name, menu);
        }

        initialized = true;
    }
}
