using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlottableAbstract : Interactuable, Slottable
{

    [SerializeField]
    protected GameObject[] slots;

    [SerializeField]
    private string menuName = "Beer basic";

    protected bool isOpened = false;

    public void Close() { isOpened = false; }

    public virtual bool IsEmpty()
    {
        if (slots == null) return true;

        foreach (var g in slots)
        {
            if (g != null) return false;
        }

        return true;
    }

    public GameObject[] GetSlots()
    {
        return slots;
    }

    public void SetSlots(GameObject[] slots)
    {
        this.slots = slots;
    }

    public GameObject Retrieve(int index)
    {
        return slots[index];
    }

    public void Slot(int index, GameObject g)
    {
        slots[index] = g;
    }

    public static List<SlottableAbstract> slottables;

    public override bool Interact(CharacterAbstract character)
    {
        // Open crafting menu with recipes
        CraftingController.OpenMenu(menuName, slots);
        isOpened = true;
        CraftingController.openedSlottable = this;
        return true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (slottables == null)
        {
            slottables = new List<SlottableAbstract>();
        }

        if (!slottables.Contains(this))
        {
            slottables.Add(this);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        slottables?.Remove(this);
    }
}
