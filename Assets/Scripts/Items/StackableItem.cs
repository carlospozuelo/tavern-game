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
    private string itemName;

    public int GetStacks() {  return currentStacks; }

    public bool IncrementStacks() {


        if (currentStacks < maxStacks) { 
            currentStacks++;
            InventoryUI.instance.UpdateUI();
            return true; 
        }

        return false;
    }

    public void SetStacks(int stacks)
    {
        currentStacks = Mathf.Min(stacks, maxStacks);
    }

    [SerializeField]
    private GameObject prefab;
    public void CancelSelectItem()
    {
    }

    public string GetName()
    {
        return itemName;
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
