using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StackableString : InventoryString
{

    [JsonProperty]
    private int currentStacks;
    [JsonProperty]
    private string stackableId;

    [JsonProperty]
    private List<string> ingredientsUsed;

    public List<string> GetIngredientsUsed() { return ingredientsUsed; }

    public string GetStackableId() { return stackableId; }

    public int GetCurrentStacks() { return currentStacks; }

    [JsonConstructor]
    public StackableString(int stacks, string id, List<String> ingredientsUsed) : base("StackableItem")
    {
        stackableId = id;
        currentStacks = stacks;
        this.ingredientsUsed = ingredientsUsed;
    }

    public StackableString(int stacks, string id, List<Ingredient> ingredientsUsed) : base("StackableItem")
    {
        stackableId = id;
        currentStacks = stacks;
        this.ingredientsUsed = new List<string>();

        foreach (var ingredient in ingredientsUsed)
        {
            this.ingredientsUsed.Add(ingredient.ingredientName);
        }
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
