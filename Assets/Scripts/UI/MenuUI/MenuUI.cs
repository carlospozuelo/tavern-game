using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField]
    protected DragDrop[] inventorySlots;
    public GameObject menu;

    protected bool isOpen = false;

    protected void UpdateImage(DragDrop[] list)
    {
        foreach (var d in list) { d.UpdateImage(); }
    }

    public virtual void UpdateImage()
    {
        UpdateImage(inventorySlots);
    }

    public void OpenOrCloseMenu()
    {
        if (!isOpen)
        {
            Open();
        }
        else { Close(); }
    }

    public virtual void Open()
    {
        // Close all other menus
        CraftingController.CloseAllMenus(1f);
        CraftingController.anyOpen = true;
        isOpen = true;
        // Stop time - temporary, should be its own script
        Time.timeScale = 0f;

        // Make all boxes in the inventory big
        InventoryUI.SelectAllItems();

        // Disable furniture preview
        FurniturePreview.instance.DisablePreview();

        PlayerInventory.instance.Disable();

        // Update all slots
        UpdateImage();

        menu.SetActive(isOpen);
        CraftingController.SetOpenedMenu(this);
    }

    public virtual void Close(float timescale = 1f)
    {
        isOpen = false;
        CraftingController.anyOpen = false;
        // Resume time - temporary, should be its own script
        Time.timeScale = timescale;

        // Select the previously selected inventory item
        InventoryUI.DeselectAllItems();
        PlayerInventory.SelectItem();

        // Enable furniture preview
        FurniturePreview.instance.EnablePreview();

        PlayerInventory.instance.Enable();

        DraggableIcon.HideImage();

        menu.SetActive(isOpen);

        DraggableIcon.HideSecondTooltip();
    }
}
