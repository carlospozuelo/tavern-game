using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Generic Recipe", menuName = "ScriptableObjects/Generic Recipe", order = 1)]
public class GenericRecipe : AbstractRecipe
{
    [Tooltip("The value of the generated item will be the sum of the values of all ingredients used, multiplied by their multiplier")]
    public List<IngredientTypeWrapper> ingredients;

    // Crafts ONE item
    public override CraftingResult Craft(List<Ingredient> list)
    {
        float value = 0;

        Tuple<Ingredient, bool>[] aux = new Tuple<Ingredient, bool>[list.Count];
        List<Ingredient> ingredientsUsed = new List<Ingredient>();

        for (int i = 0; i < list.Count; i++)
        {
            aux[i] = new Tuple<Ingredient, bool>(list[i], false);
        }

        // Go through all of the ingredients, and see if they're present on the recipe
        foreach (IngredientTypeWrapper ingredientType in ingredients)
        {
            bool found = false;
            for (int i = 0; i < aux.Length; i++)
            {
                if (aux[i].Item2) { continue; }

                if (aux[i].Item1.HasAllTypes(ingredientType.type))
                {
                    // It's a valid ingredient
                    value += (aux[i].Item1.value * ingredientType.valueMultiplier);
                    found = true;

                    // Mark the item to used, so that the same item can't be used twice per recipe.
                    aux[i] = new Tuple<Ingredient, bool>(aux[i].Item1, true);

                    ingredientsUsed.Add(aux[i].Item1);
                    break;
                }
            }

            if (!found)
            {
                // Some ingredients weren't found in the recipe -> Abort
                return null;
            }
        }

        return new CraftingResult(
            result.ingredient.name,
            value,
            ingredientsUsed,
            result.amount);
        
    }
}

[System.Serializable]
public class IngredientTypeWrapper
{
    public Ingredient.IngredientType type;
    public float valueMultiplier = 1;
}
