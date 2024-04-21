using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackableItem : MonoBehaviour, Item
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private int maxStacks = 30, currentStacks = 1;

    [SerializeField]
    private Ingredient ingredient;

    [SerializeField]
    private List<Ingredient> madeOf;
    public void SetIngredients(List<Ingredient> list) { madeOf = list; }
    public List<Ingredient> GetIngredients() { return madeOf; }

    [SerializeField]
    private float value = 0;

    public void SetValue(float value) { this.value = value; }
    public float GetValue() { return value; }

    public void SetIngredient(Ingredient ingredient)
    {
        this.ingredient = ingredient;
        this.sprite = ingredient.sprite;
    }

    public Ingredient GetIngredient() { return ingredient; }

    public int GetStacks() {  return currentStacks; }

    public bool IsFull() { return maxStacks == currentStacks; }

    public bool CanStack(StackableItem stackableItem, bool useCurrentStacks = false)
    {
        return CanStack(stackableItem.GetName(), useCurrentStacks ? stackableItem.currentStacks : 0, stackableItem.madeOf);
    }

    // Returns true if they have the same name, value and list of ingredients
    public bool SoftEquals(StackableItem other)
    {
        if (other == null) return false;

        if (!other.GetName().Equals(GetName())) { return false; }

        if (other.value != value) return false; 

        if (other.GetIngredients().Count != madeOf.Count) { return false; }

        foreach (var ingredient in madeOf)
        {
            if (!other.madeOf.Contains(ingredient)) { return false; }
        }

        return true;
    }

    public bool CanStack(string name, int stacks, List<Ingredient> ingredients)
    {
        if (!name.Equals(GetName())) { return false; }
        if (stacks + this.currentStacks > maxStacks) { return false; }

        if (ingredients.Count != madeOf.Count) { return false; }
        foreach (var ingredient in madeOf)
        {
            if (!ingredients.Contains(ingredient)) { return false; }
        }

        return true;
    }
    public bool IncrementStacks() {


        if (currentStacks < maxStacks) { 
            currentStacks++;
            InventoryUI.instance.UpdateUI();
            return true; 
        }
        return false;
    }

    // Warning: this method destroys the stackable item.
    public bool Consume()
    {
        currentStacks--;

        if (currentStacks <= 0)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    public int IncrementStacks(int amount)
    {
        if (currentStacks + amount <= maxStacks)
        {
            currentStacks += amount;
            InventoryUI.instance.UpdateUI();
            return 0;
        }

        int leftover = (currentStacks + amount) - maxStacks;
        currentStacks = maxStacks;

        InventoryUI.instance.UpdateUI();

        return leftover;
    }

    public void SetStacks(int stacks)
    {
        currentStacks = Mathf.Min(stacks, maxStacks);
        InventoryUI.instance.UpdateUI();
    }

    [SerializeField]
    private GameObject prefab;
    public void CancelSelectItem()
    {
    }

    public string GetName()
    {
        return ingredient.ingredientName;
    }

    public GameObject GetOriginalPrefab()
    {
        return prefab;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    public void SelectItem()
    {
    }

    public bool UseItem()
    {
        return false;
    }

    public override string ToString()
    {
        string str = "{name: " + GetName() + ", value: " + GetValue() + ", ingredients:[";

        foreach (Ingredient i in madeOf)
        {
            str += i + ", ";
        }

        str += "]}";

        return str;
    }

    public string GetDescription()
    {
        string str = "(";
        for (int i = 0; i < madeOf.Count - 1; i++) {
            str += madeOf[i].ingredientName + ", ";
        }

        if (madeOf.Count > 0)
        {
            str += madeOf[madeOf.Count - 1].ingredientName + ")";

            return str;
        } else
        {
            return "";
        }
    }
}
