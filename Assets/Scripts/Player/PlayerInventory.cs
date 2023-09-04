using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public Item[] hotBar;
    public Item[] inventory;

    public int currentItem = 0;

    InventoryUI inventoryUI;

    // Start is called before the first frame update
    void Start()
    {
        hotBar = new Item[10];
        inventory = new Item[30];

        inventoryUI = InventoryUI.instance;
    }

    public void NextItem()
    {
        currentItem++;
        currentItem = currentItem % 10;

        inventoryUI.UpdateUI(currentItem);
    }

    public void PreviousItem()
    {
        currentItem--;

        if (currentItem == -1)
        {
            currentItem = 9;
        }

        inventoryUI.UpdateUI(currentItem);
    }

    public void SelectItem(int item)
    {
        currentItem = item;

        inventoryUI.UpdateUI(currentItem);
    }

    private KeyCode[] keyCodes = {
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };
    public void Update()
    {
        // TODO: Should be on a separate script
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            if (wheel > 0) { PreviousItem(); } else { NextItem(); }
        }

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                SelectItem(i);
            }
        }
    }
}
