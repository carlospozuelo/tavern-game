using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public GameObject[] hotBar;
    public GameObject[] inventory;

    public int currentItem = 0;

    public static PlayerInventory instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < hotBar.Length; i++)
        {
            GameObject g = hotBar[i];
            if (g != null)
            {
                InventoryUI.instance.UpdateSpriteHotbar(g.GetComponent<Item>(), i);
            }
        }
    }

    // Start is called before the first frame update

    public void NextItem()
    {
        currentItem++;
        currentItem = currentItem % 10;

        InventoryUI.instance.UpdateUI(currentItem);
    }

    public void PreviousItem()
    {
        currentItem--;

        if (currentItem == -1)
        {
            currentItem = 9;
        }

        InventoryUI.instance.UpdateUI(currentItem);
    }

    public void SetHotBar(int slot, GameObject item)
    {
        hotBar[slot] = item;
    }

    public void SetInventory(int slot, GameObject item)
    {
        inventory[slot] = item;
    }

    public void SelectItem(int item)
    {
        currentItem = item;

        InventoryUI.instance.UpdateUI(currentItem);
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

        if (Input.GetMouseButtonDown(0)) { UseItem(); }
    }

    public void UseItem()
    {
        if (hotBar[currentItem] != null)
        {
            hotBar[currentItem].GetComponent<Item>().UseItem();
        }
    }

    public void ConsumeItem()
    {
        hotBar[currentItem] = null;
        InventoryUI.instance.UpdateSpriteHotbar(null, currentItem);
    }
}
