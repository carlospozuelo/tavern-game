using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public GameObject[] hotBar;
    public GameObject[] inventory;

    public int currentItem = 0;

    public static PlayerInventory instance;

    public GameObject GetCurrentItem()
    {
        return hotBar[currentItem];
    }

    public void SetCurrentItem(GameObject g)
    {
        if (g != null || hotBar[currentItem] != null)
        {
            hotBar[currentItem] = g;

            SelectItem(currentItem);
            
        } else
        {
            Debug.LogWarning("Tried to override item (" + currentItem + ")!");
        }
    }

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

        SelectItem(1);
    }

    // Start is called before the first frame update

    public void NextItem()
    {
        int currentItem = this.currentItem + 1;
        currentItem = currentItem % 10;

        SelectItem(currentItem);
    }

    public void PreviousItem()
    {
        int currentItem = this.currentItem - 1;

        if (currentItem == -1)
        {
            currentItem = 9;
        }

        SelectItem(currentItem);
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
        if (hotBar[currentItem] != null)
        {
            hotBar[currentItem].GetComponent<Item>().CancelSelectItem();
        }

        currentItem = item;

        InventoryUI.instance.UpdateUI(currentItem);
        if (hotBar[currentItem] != null)
        {
            hotBar[currentItem].GetComponent<Item>().SelectItem();
        }
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

        if (Input.GetMouseButtonDown(0)) { 
            if (!UseItem()) { 
                foreach (GameObject furniture in TavernController.GetPlacedFurnitures())
                {
                    Furniture f = furniture.GetComponent<Furniture>();
                    if (f.IsInsideObject(GameController.instance.WorldMousePosition())) {
                        f.PickUp();
                        break;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject item = GetCurrentItem();
            if (item != null && item.TryGetComponent(out Furniture f))
            {
                if (f.rotateGameObject != null)
                {
                    SetCurrentItem(f.rotateGameObject);
                    InventoryUI.instance.UpdateSpriteHotbar(f.rotateGameObject.GetComponent<Item>(), currentItem);
                }
            }
        }
    }

    public bool UseItem()
    {
        if (hotBar[currentItem] != null)
        {
            hotBar[currentItem].GetComponent<Item>().UseItem();
            return true;
        } else
        {
            return false;
        }
    }

    public void ConsumeItem()
    {
        hotBar[currentItem] = null;
        InventoryUI.instance.UpdateSpriteHotbar(null, currentItem);
    }
}
