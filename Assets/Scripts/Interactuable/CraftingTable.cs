using UnityEngine;

// This class represents an interactable furniture that opens a crafting menu
public class CraftingTable : Interactuable
{
    [SerializeField]
    private string menuName = "Beer basic";
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
        CraftingController.OpenMenu(menuName);
        return true;
    }
}
