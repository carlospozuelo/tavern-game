using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe", order = 1)]
public class CraftingRecipe : AbstractRecipe
{
    public List<Ingredient> ingredients;

    public override float CalculateValue(List<Ingredient> list)
    {
        throw new NotImplementedException();
    }

    public override CraftingResult Craft(List<Ingredient> list)
    {
        throw new NotImplementedException();
    }

    public float GetResultValue()
    {
        float value = 0;

        foreach (var ingredient in ingredients)
        {
            value += ingredient.value;
        }

        return value;
    }
}


