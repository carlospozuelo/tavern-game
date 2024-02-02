using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMenuUI : MonoBehaviour
{
    private static BookMenuUI instance;

    private bool isOpen = false;
    public GameObject menu;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private DragDrop[] slots, clothingSlots;

    public static Canvas GetCanvas()
    {
        return instance.canvas;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }

    public static void OpenOrCloseMenu()
    {
        instance.isOpen = !instance.isOpen;

        if (instance.isOpen)
        {
            Open();
        } else { Close(); }

        instance.menu.SetActive(instance.isOpen);
    }

    private static void Open()
    {
        // Stop time - temporary, should be its own script
        Time.timeScale = 0f;

        // Make all boxes in the inventory big
        InventoryUI.SelectAllItems();

        // Disable furniture preview
        FurniturePreview.instance.DisablePreview();

        PlayerInventory.instance.Disable();

        // Update all slots
        UpdateImage();
    }

    public static void UpdateImage()
    {
        UpdateImage(instance.slots);
        UpdateImage(instance.clothingSlots);
    }

    private static void UpdateImage(DragDrop[] list)
    {
        foreach (var d in list) { d.UpdateImage(); }
    }

    private static void Close()
    {
        // Resume time - temporary, should be its own script
        Time.timeScale = 1f;

        // Select the previously selected inventory item
        InventoryUI.DeselectAllItems();
        PlayerInventory.SelectItem();

        // Enable furniture preview
        FurniturePreview.instance.EnablePreview();

        PlayerInventory.instance.Enable();

        DraggableIcon.HideImage();
    }

    public static bool IsOpen() { return instance.isOpen; }
}
