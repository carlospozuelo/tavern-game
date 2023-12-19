using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMenuUI : MonoBehaviour
{
    private static BookMenuUI instance;

    private bool isOpen = false;
    public GameObject menu;

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
    }

    private static void Close()
    {
        // Resume time - temporary, should be its own script
        Time.timeScale = 1f;

        // Select the previously selected inventory item
        InventoryUI.DeselectAllItems();
        PlayerInventory.SelectItem();
    }
}
