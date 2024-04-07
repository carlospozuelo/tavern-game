using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/Ingredient", order = 1)]
public class Ingredient : ScriptableObject
{
    public string ingredientName;

    [SerializeField]
    private IngredientType type;

    public List<IngredientType> GetAllTypes()
    {
        List<IngredientType> list = new List<IngredientType>();

        foreach (IngredientType type in Enum.GetValues(typeof(IngredientType)))
        {
            if ((this.type & type) == type && this.type != 0)
            {
                list.Add(type);
            } 
        }

        return list;
    }

    public bool HasAnyType(IngredientType type)
    {
        return (this.type & type) != 0;
    }

    public bool HasAllTypes(IngredientType type)
    {
        return (this.type & type) == type;
    }

    [Flags]
    public enum IngredientType {
        None = 0,
        Grain = 1,
        Plant = 2,
        Drink = 4,
        Resource = 8
    }

    public Sprite sprite;

    [Tooltip("Value of the ingredient relative to the type. Affects the price of generic recipes")]
    public float value = 1;

    public Ingredient(string name)
    {
        this.name = name;
    }
}

[System.Serializable]
public class IngredientWrapper
{
    public Ingredient ingredient;
    public int amount;
}
