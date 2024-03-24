using UnityEngine;

// This class represents an interactable furniture that opens a crafting menu
public class CraftingTable : Interactuable, Slottable
{
    [SerializeField]
    private string menuName = "Beer basic";

    [SerializeField]
    private GameObject[] slots;

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
