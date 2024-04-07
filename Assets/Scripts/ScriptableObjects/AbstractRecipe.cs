using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRecipe : ScriptableObject
{
    public CraftingController.Tables requiredTable;

    public IngredientWrapper result;

    public abstract GameObject Craft(List<Ingredient> list);

}
