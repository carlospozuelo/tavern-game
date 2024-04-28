using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This controller contains every item that is available and that the npcs can demand.
public class TavernStockController : MonoBehaviour
{
    private static TavernStockController instance;

    private Dictionary<Ingredient.IngredientType, List<StackableItem>> availableIngredients;
    private List<Ingredient> availableIngredientList;

    public Ingredient.IngredientType desiredTypes;

    // Ask for ANY type of beer. Not a specific one
    public static Ingredient GetRandomIngredient()
    {
        if (instance.availableIngredientList.Count > 0)
        {
            return instance.availableIngredientList[Random.Range(0, instance.availableIngredientList.Count)];
        }

        return null;
    }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    private void Start()
    {
        Initialize();
    }

    private bool initialized = false;

    private void RegenerateDictionary()
    {
        availableIngredients = new Dictionary<Ingredient.IngredientType, List<StackableItem>>();
        availableIngredientList = new List<Ingredient>();
        foreach (Transform child in GameController.instance.stackableParent)
        {
            StackableItem item = child?.GetComponent<StackableItem>();
            Ingredient ingredient = item?.GetIngredient();

            if (ingredient != null)
            {
                if (ingredient.HasAnyType(desiredTypes))
                {
                    if (!availableIngredientList.Contains(ingredient))
                    {
                        availableIngredientList.Add(ingredient);
                    }
                    // Add to the list
                    foreach (Ingredient.IngredientType type in ingredient.GetAllTypes()) { 
                        // If the type is one of the desired types, add it to the dictionary
                        if ((type & desiredTypes) != 0)
                        {
                            if (!availableIngredients.ContainsKey(type))
                            {
                                availableIngredients.Add(type, new List<StackableItem>());
                            }

                            bool add = true;
                            foreach (StackableItem i in availableIngredients[type])
                            {
                                if (i.SoftEquals(item))
                                {
                                    add = false;
                                    break;
                                }
                            }

                            if (add)
                            {
                                availableIngredients[type].Add(item);
                            }
                        }
                    }
                }
            }
        }

        PrintDictionaryDebug();
    }

    private void PrintDictionaryDebug()
    {
        string str = "{\n";
        foreach (var key in  availableIngredients.Keys) {
            str += key + ":[";
            foreach(var val in availableIngredients[key])
            {
                str += val + ",";
            }
            str += "],\n";
        }
        str += "}";

        print(str);
    }

    private void Initialize()
    {
        if (initialized) return;
        initialized = true;

        RegenerateDictionary();
    }

    
}
