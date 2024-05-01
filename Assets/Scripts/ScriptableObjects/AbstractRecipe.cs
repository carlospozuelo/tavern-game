using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRecipe : ScriptableObject
{
    public CraftingController.Tables requiredTable;

    public IngredientWrapper result;

    public bool consumesIngredient = true;

    public abstract CraftingResult Craft(List<Ingredient> list);

    public abstract float CalculateValue(List<Ingredient> list);

}

public class CraftingResult
{
    public string ingredientName;
    public float value;

    public List<Ingredient> ingredientsUsed;
    public int stacks;

    public CraftingResult(string ingredientName, float value, List<Ingredient> ingredientsUsed, int stacks)
    {
        this.ingredientName = ingredientName;
        this.value = value;
        this.ingredientsUsed = ingredientsUsed;
        this.stacks = stacks;
    }
}
