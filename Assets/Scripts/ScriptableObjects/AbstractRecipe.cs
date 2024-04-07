using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRecipe : ScriptableObject
{
    public CraftingController.Tables requiredTable;

    public IngredientWrapper result;

    public abstract StackableItem Craft(List<Ingredient> list, DragDrop slot);

}
