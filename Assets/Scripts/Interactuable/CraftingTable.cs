using System.Collections.Generic;
using UnityEngine;

// This class represents an interactable furniture that opens a crafting menu
public class CraftingTable : Interactuable, Slottable
{
    [SerializeField]
    private string menuName = "Beer basic";

    [SerializeField]
    private GameObject[] slots;

    public static List<CraftingTable> craftingTables;

    public CraftingController.Tables tableType;

    public float delay = 4f;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (craftingTables == null)
        {
            craftingTables = new List<CraftingTable>();
        }

        if (!craftingTables.Contains(this))
        {
            craftingTables.Add(this);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        craftingTables.Remove(this);
    }

    private bool isOpened = false;
    public void Close() { isOpened = false; }

    public override bool CanBeUsedByNPCS()
    {
        // Only for player
        return false;
    }

    public override GameObject GetGameObject()
    {
        return gameObject;
    }

    public override Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    public override bool Interact(CharacterAbstract character)
    {
        // Open crafting menu with recipes
        CraftingController.OpenMenu(menuName, slots);
        isOpened = true;
        CraftingController.openedCraftingTable = this;
        return true;
    }

    public GameObject Retrieve(int index)
    {
        return slots[index];
    }

    public void Slot(int index, GameObject g)
    {
        slots[index] = g;
    }

    public bool IsEmpty()
    {
        if (slots == null) return true;

        foreach (var g in slots)
        {
            if (g != null) return false;
        }

        return true;
    }

}
