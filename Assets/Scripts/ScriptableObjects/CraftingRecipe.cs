using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe", order = 1)]
public class CraftingRecipe : AbstractRecipe
{
    public List<Ingredient> ingredients;

}


