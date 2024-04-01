using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/Ingredient", order = 1)]
public class Ingredient : ScriptableObject
{
    public string ingredientName;

    public IngredientType type;
    public enum IngredientType { Grain, Plant, Drink, Resource }
    public Sprite sprite;

    [Tooltip("Value of the ingredient relative to the type. Affects the price of generic recipes")]
    public int value = 1;

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
