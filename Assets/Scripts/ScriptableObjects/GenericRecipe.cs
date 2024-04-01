using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Generic Recipe", menuName = "ScriptableObjects/Generic Recipe", order = 1)]
public class GenericRecipe : AbstractRecipe
{
    public List<Ingredient.IngredientType> ingredients;
}
