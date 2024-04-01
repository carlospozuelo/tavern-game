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

    public string GetStackableId() { return stackableId; }

    public int GetCurrentStacks() { return currentStacks; }

    public StackableString(int stacks, string id) : base("StackableItem")
    {
        stackableId = id;
        currentStacks = stacks;
    }
}
