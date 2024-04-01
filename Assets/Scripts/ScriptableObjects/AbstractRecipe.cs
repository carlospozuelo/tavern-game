using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractRecipe : ScriptableObject
{
    public CraftingController.Tables requiredTable;

    public IngredientWrapper result;
}
