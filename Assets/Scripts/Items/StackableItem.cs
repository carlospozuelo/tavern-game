using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackableItem : MonoBehaviour, Item
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private int maxStacks = 30, currentStacks = 0;
    [SerializeField]
    private string itemName;

    public int GetStacks() {  return currentStacks; }

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
