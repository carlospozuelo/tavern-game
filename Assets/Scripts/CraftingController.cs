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

    public static SlottableAbstract openedSlottable;

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
            if (index < 0 || index >= instance.slots.Length) { 
                
                return null; 
            }
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
                // Attempt to craft
                instance.CheckCrafts();
            }
        }
    }

    private Dictionary<CraftingTable, Coroutine> coroutines;
    private Dictionary<CraftingTable, List<Ingredient>> initialIngredientsByTable;

    private IEnumerator Timer(CraftingTable table)
    {
        float elapsedTime = 0f;
        while (elapsedTime < table.delay) {
            if (table == openedSlottable)
            {
                CraftingMenuUI.SetProgress(table.delay, elapsedTime);
            }

            yield return new WaitForSecondsRealtime(.1f);
            elapsedTime += .1f;
        }
    }

    private IEnumerator CraftCoroutine(GameObject[] slots, CraftingTable currentTable)
    {
        List<Ingredient> currentIngredients = new List<Ingredient>();
        List<StackableItem> stackablesUsed = new List<StackableItem>();

        for (int index = 0; index < slots.Length - 1; index++)
        {
            GameObject g = slots[index];
            if (g != null && g.TryGetComponent(out StackableItem i))
            {
                currentIngredients.Add(i.GetIngredient());
                stackablesUsed.Add(i);
            }
        }

        if (initialIngredientsByTable.ContainsKey(currentTable))
        {
            initialIngredientsByTable.Remove(currentTable);
            Debug.LogWarning("Key: " + currentTable + " was present in the initialIngredientsByTable dictionary, when it shouldn't have.");
        }
        initialIngredientsByTable.Add(currentTable, currentIngredients);

        foreach (AbstractRecipe recipe in instance.dictionary[currentTable.tableType])
        {
            CraftingResult craftingResult = recipe.Craft(currentIngredients);
            if (craftingResult != null)
            {
                bool canStack = slots[slots.Length - 1] == null;
                int stacks = craftingResult.stacks;
                // 2. Only assign the item there if the slot is vacant. If it's not, and the recipe produces the EXACT same stackable item, stack instead. Otherwise, pause crafting
                if (!canStack)
                {
                    if (slots[slots.Length - 1].TryGetComponent(out StackableItem currentStackable) && currentStackable.CanStack(craftingResult.ingredientName, craftingResult.stacks, craftingResult.ingredientsUsed))
                    {
                        canStack = true;
                    }
                }

                if (canStack)
                {
                    while (true)
                    {
                        // Can be crafted -> Wait and then slot
                        // Maybe not the best approach, as if the game is actually paused, recipes will continue
                        yield return Timer(currentTable);
                        if (slots[slots.Length -1] == null)
                        {
                            slots[slots.Length - 1] = GameController.GenerateStackableItem(craftingResult.ingredientName, craftingResult.ingredientsUsed, stacks);
                        } else
                        {
                            slots[slots.Length - 1].GetComponent<StackableItem>().IncrementStacks(stacks);
                        }
                        // Consume ingredient
                        bool abort = false;
                        if (recipe.consumesIngredient)
                        {
                            
                            foreach (StackableItem item in stackablesUsed)
                            {
                                if (item.Consume()) { abort = true; };
                            }
                        }

                        yield return null;

                        if (openedMenu != null)
                        {
                            openedMenu.UpdateImage();
                        }

                        if (abort) {
                            initialIngredientsByTable.Remove(currentTable);
                            yield break;
                        }
                    }
                }
            }
        }
    }

    public static void CheckCurrentCrafts() {
        instance.CheckCrafts();
    }

    private bool CompareIngredients(List<Ingredient> ing)
    {
        if (ing != null)
        {
            int count = 0;
            for (int index = 0; index < slots.Length - 1; index++)
            {
                GameObject g = slots[index];
                if (g != null && g.TryGetComponent(out StackableItem i))
                {
                    Ingredient ingredient = i.GetIngredient();
                    count++;
                    if (!ing.Contains(ingredient))
                    {
                        return false;
                    }
                }
            }

            if (count == ing.Count)
            {
                return true;
            }
        }

        return false;
    }

    private void CheckAllCraftsPriv()
    {
        foreach (SlottableAbstract slottable in SlottableAbstract.slottables)
        {
            if (slottable is CraftingTable)
            {
                CheckCraftsPriv((CraftingTable) slottable);
            }
        }
    }

    public static void CheckAllCrafts()
    {
        instance.CheckAllCraftsPriv();
    }

    private void CheckCraftsPriv(CraftingTable table)
    {
        if (coroutines.ContainsKey(table))
        {
            if (coroutines[table] != null)
            {
                StopCoroutine(instance.coroutines[table]);
                initialIngredientsByTable.Remove(table);
            }

            coroutines.Remove(table);
        }

        coroutines.Add(table, StartCoroutine(CraftCoroutine(table.GetSlots(), table)));

    }

    public static void CheckCrafts(CraftingTable table)
    {
        instance.CheckCraftsPriv(table);
    }

    // Check if, with the current ingredients, something can be crafted. If so, craft
    private void CheckCrafts()
    {
        bool startCoroutine = true;
        if (!(openedSlottable is CraftingTable)) { return; }

        CraftingTable openedCraftingTable = (CraftingTable) openedSlottable;

        if (coroutines.ContainsKey(openedCraftingTable))
        {
            if (coroutines[openedCraftingTable] != null)
            {
                if (initialIngredientsByTable.TryGetValue(openedCraftingTable, out var list) && !CompareIngredients(list))
                {
                    StopCoroutine(coroutines[openedCraftingTable]);
                    initialIngredientsByTable.Remove(openedCraftingTable);
                    coroutines.Remove(openedCraftingTable);
                } else
                {
                    // They are the same ingredients. No need to restart the craft.
                    startCoroutine = false;
                }
            } else
            {
                initialIngredientsByTable.Remove(openedCraftingTable);
                coroutines.Remove(openedCraftingTable);
            }

        }

        if (startCoroutine && slots != null)
        {

            coroutines.Add(openedCraftingTable, StartCoroutine(CraftCoroutine(slots, openedCraftingTable)));
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
        foreach (SlottableAbstract s in SlottableAbstract.slottables)
        {
            s.Close();
        }

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
        initialIngredientsByTable = new Dictionary<CraftingTable, List<Ingredient>>();
        coroutines = new Dictionary<CraftingTable, Coroutine>();
        initialized = true;
    }
}
