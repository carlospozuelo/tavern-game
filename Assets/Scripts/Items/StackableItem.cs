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
    public bool IncrementStacks() {


        if (currentStacks < maxStacks) { 
            currentStacks++;
            InventoryUI.instance.UpdateUI();
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
}
