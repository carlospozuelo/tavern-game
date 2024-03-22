using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe", order = 1)]
public class CraftingRecipe : ScriptableObject
{

    [Serializable]
    public class Ingredient
    {
        public string name;
        public int amount;

        public Ingredient(string name, int amount)
        {
            this.name = name;
            this.amount = amount;
        }
    }

    public CraftingController.Tables requiredTable;

    public List<Ingredient> ingredients;

    public Ingredient result;


}


