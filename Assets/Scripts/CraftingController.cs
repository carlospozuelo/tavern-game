using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is meant to be a controller to store different types of crafting menus and recipes
public class CraftingController : MonoBehaviour
{
    public enum Tables { Basic_Beer_Tap }

    [SerializeField]
    private AbstractRecipe[] recipes;
    [SerializeField]
    private MenuUI[] allMenus;

    [SerializeField]
    private Ingredient[] allIngredients;

    private MenuUI openedMenu;

    public static void SetOpenedMenu(MenuUI menu)
    {
        instance.openedMenu = menu;
    }

    public void SetAllIngreidents(Ingredient[] ingredients) { this.allIngredients = ingredients; }

    public void SetAllRecipes(AbstractRecipe[] recipes) { this.recipes = recipes; }

    private Dictionary<Tables, List<AbstractRecipe>> dictionary;
    private Dictionary<string, MenuUI> menus;

    private Dictionary<string, Ingredient> ingredientDictionary;
    private Dictionary<Ingredient.IngredientType, List<Ingredient>> ingredientTypeList;

    private static CraftingController instance;

    private bool initialized = false;

    public static bool anyOpen = false;

    private GameObject[] slots;

    public static Ingredient GetIngredient(string name)
    {
        return instance.ingredientDictionary[name];
    }

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

        dictionary = new Dictionary<Tables, List<AbstractRecipe>>();
        menus = new Dictionary<string, MenuUI>();
        ingredientDictionary = new Dictionary<string, Ingredient>();
        ingredientTypeList = new Dictionary<Ingredient.IngredientType, List<Ingredient>>();

        foreach (Tables type in System.Enum.GetValues(typeof (Tables)))
        {
            dictionary.Add(type, new List<AbstractRecipe>());
        }

        foreach (AbstractRecipe recipe in recipes)
        {
            dictionary[recipe.requiredTable].Add(recipe);
        }

        foreach (MenuUI menu in allMenus)
        {
            menus.Add(menu.menu.name, menu);
        }

        foreach (Ingredient i in allIngredients)
        {
            ingredientDictionary.Add(i.ingredientName, i);

            foreach (var type in i.GetAllTypes())
            {
                if (!ingredientTypeList.ContainsKey(type))
                {
                    ingredientTypeList.Add(type, new List<Ingredient>());
                }

                ingredientTypeList[type].Add(i);
            }
            
        }
        print("Initialized");
        initialized = true;
    }
}
