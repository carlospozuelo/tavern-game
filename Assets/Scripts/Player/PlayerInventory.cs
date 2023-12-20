using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class InventoryData
{
    [JsonProperty]
    private string[] inventory, hotbar;

    public InventoryData()
    {
        inventory = new string[20];
        hotbar = new string[10];
    }

    public InventoryData(GameObject[] inventory, GameObject[] hotbar)
    {
        this.inventory = new string[inventory.Length];
        this.hotbar = new string[hotbar.Length];

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
            {
                GameObject g = inventory[i];
                if (g.TryGetComponent(out Item item))
                {
                    this.inventory[i] = item.GetName();
                }
                else
                {
                    Debug.LogError("Trying to read an item out of " + g + ", but it was not found. Inventory should only contain Items.");
                }
            }
        }

        for(int i = 0; i < hotbar.Length; i++)
        {
            if (hotbar[i] != null)
            {
                GameObject g = hotbar[i];
                if (g.TryGetComponent(out Item item))
                {
                    this.hotbar[i] = item.GetName();
                }
                else
                {
                    Debug.LogError("Trying to read an item out of " + g + ", but it was not found. Hotbar should only contain Items.");
                }
            }
        }
    }

    public GameObject[] GetInventory()
    {
        GameObject[] inventory = new GameObject[this.inventory.Length];

        for (int i = 0; i < inventory.Length; i++)
        {
            if (!string.IsNullOrEmpty(this.inventory[i])) {
                inventory[i] = PlayerInventory.instance.GetItem(this.inventory[i]);
            }
        }

        return inventory;
    }

    public GameObject[] GetHotbar()
    {
        GameObject[] hb = new GameObject[this.hotbar.Length];

        for (int i = 0; i < hotbar.Length; i++)
        {
            if (!string.IsNullOrEmpty(this.hotbar[i]))
            {
                hb[i] = PlayerInventory.instance.GetItem(this.hotbar[i]);
            }
        }

        return hb;
    }
}


public class PlayerInventory : MonoBehaviour
{

    public GameObject[] hotBar;
    public GameObject[] inventory;

    [SerializeField]
    // This does NOT include furniture. Furniture are pulled from the Furniture controller.
    private GameObject[] allItems;

    // This includes both Items and Furniture.
    private Dictionary<string, GameObject> allItemsDictionary;

    public int currentItem = 0;

    public static PlayerInventory instance;

    public GameObject GetCurrentItem()
    {
        return hotBar[currentItem];
    }

    public GameObject GetItem(string name)
    {
        if (allItemsDictionary.ContainsKey(name))
        {
            return allItemsDictionary[name];
        }

        Debug.LogError("Error: " + name + " not found in the dictionary.");

        return null;
    }

    private bool listening = true;
    public void Enable() { listening = true; }
    public void Disable() { listening = false; }

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

    private void InitializeDictionary()
    {
        allItemsDictionary = new Dictionary<string, GameObject>();

        foreach (GameObject g in allItems)
        {
            allItemsDictionary.Add(g.name, g);
        }

        // Also add furniture
        foreach (GameObject g in TavernController.GetAllFurnitureRaw())
        {
            allItemsDictionary.Add(g.name, g);
        }
    }

    private void Start()
    {
        InitializeDictionary();
        Deserialize();
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
        InventoryUI.instance.UpdateUI(slot);
    }

    public void SetInventory(int slot, GameObject item)
    {
        inventory[slot] = item;
    }

    public static void SelectItem()
    {
        instance.SelectItem(instance.currentItem);
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
        if (listening)
        {
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

            if (Input.GetMouseButtonDown(0))
            {
                if (!UseItem())
                {
                    List<Furniture> list = new List<Furniture>();
                    foreach (GameObject furniture in TavernController.GetPlacedFurnitures())
                    {
                        Furniture f = furniture.GetComponent<Furniture>();
                        if (f.IsInsideObject(GameController.instance.WorldMousePosition()))
                        {
                            list.Add(f);
                        }
                    }


                    Furniture toBePickedUp = null;
                    foreach (Furniture f in list)
                    {
                        if (f.itemsOnTop.Count == 0 && !f.IsBlocked())
                        {
                            toBePickedUp = f;
                            if (!f.rugLike)
                            {
                                break;
                            }
                        }

                    }

                    if (toBePickedUp != null)
                    {
                        toBePickedUp.PickUp();
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                GameObject item = GetCurrentItem();

                Interactuable i = null;
                foreach (GameObject interactuable in TavernController.GetCurrentInteractuables())
                {
                    Interactuable aux = interactuable.GetComponent<Interactuable>();

                    if (aux.IsInsideObject(GameController.instance.WorldMousePosition()))
                    {
                        i = aux;
                        break;
                    }

                }

                if (i == null)
                {
                    if (item != null && item.TryGetComponent(out Furniture f))
                    {
                        if (f.rotateGameObject != null)
                        {
                            SetCurrentItem(f.rotateGameObject);
                            InventoryUI.instance.UpdateSpriteHotbar(f.rotateGameObject.GetComponent<Item>(), currentItem);
                        }
                    }
                }
                else
                {
                    if (Vector2.Distance(gameObject.transform.position, i.GetPosition()) <= i.GetMaxDistance())
                    {
                        i.Interact();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            BookMenuUI.OpenOrCloseMenu();
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

    public InventoryData Serialize()
    {
        return new InventoryData(instance.inventory, instance.hotBar);
    }

    public void Deserialize()
    {
        InventoryData d = ReadData();

        if (d == null)
        {
            Debug.Log("No inventory data was found !");
        }
        else
        {
            inventory = d.GetInventory();
            hotBar = d.GetHotbar();

            for (int i = 0; i < hotBar.Length; i++)
            {
                InventoryUI.instance.UpdateUI(i);
            }
        }
    }

    public InventoryData ReadData()
    {
        Master m = MasterData.Read();

        if (m == null) return null;
        return m.inventoryData;
    }
}
