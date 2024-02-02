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

    public int GetCurrentStacks() { return currentStacks; }

    public StackableString(int stacks, string id) : base(id)
    {
        currentStacks = stacks;
    }
}
