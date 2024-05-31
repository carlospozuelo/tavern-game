using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public GameObject[] hotBar;
    public GameObject[] inventory;

    private Dictionary<ClothingItem.ClothingType, Clothing> clothingDictionary;

    [SerializeField]
    // This does NOT include furniture. Furniture are pulled from the Furniture controller.
    private GameObject[] allItems;

    public void SetAllItems(GameObject[] allItems) { this.allItems = allItems; }

    // This includes both Items and Furniture.
    private Dictionary<string, GameObject> allItemsDictionary;

    public int currentItem = 0;

    public static PlayerInventory instance;

    private float gold;

    public GameObject GetCurrentItem()
    {
        return hotBar[currentItem];
    }

    public static void Wear(Clothing clothing)
    {
        instance.clothingDictionary[clothing.GetClothingItem().type] = clothing;
    }

    public static Clothing GetWornItem(ClothingItem.ClothingType key)
    {
        instance.Initialize();

        if (instance.clothingDictionary.TryGetValue(key, out var value)) { return value; } return null;
    }


    public static bool StoreAnywhere(Item item)
    {
        if (item is StackableItem) {
            return StoreAnywhereStackable((StackableItem)item); 
        }

        return StoreAnywhere(item.GetOriginalPrefab());
    }

    public static bool StoreAnywhere(GameObject prefab)
    {
        for (int i = 0; i < instance.hotBar.Length; i++)
        {
            if (instance.hotBar[(i + 1) % 10] == null)
            {
                instance.SetHotBar((i + 1) % 10, prefab);
                if ((i + 1) % 10 == instance.currentItem) { SelectItem(); }
                return true;
            }
        }

        for (int i = 0; i < instance.inventory.Length; i++)
        {
            if (instance.inventory[i] == null)
            {
                instance.SetInventory(i, prefab);
                return true;
            }
        }

        return false;
    }

    private static bool StoreAnywhereStackable(StackableItem item)
    {
        // Check if the same item is already slotted in the inventory / hotbar

        for (int i = 0; i < instance.hotBar.Length; i++)
        {
            GameObject eval = instance.hotBar[(i + 1) % 10];

            if (eval != null)
            {
                if (eval.TryGetComponent(out StackableItem evalItem))
                {
                    if (item.CanStack(evalItem))
                    {
                        // We have a match - increment its stacks if we can.
                        if (evalItem.IncrementStacks()) { return true; }
                    }
                }
            }
        }

        for (int i = 0; i < instance.inventory.Length; i++)
        {
            GameObject eval = instance.inventory[i];

            if (eval != null)
            {
                if (eval.TryGetComponent(out StackableItem evalItem))
                {
                    if (evalItem.GetName().Equals(item.GetName()))
                    {
                        // We have a match - increment its stacks if we can.
                        if (evalItem.IncrementStacks()) { return true; }
                    }
                }
            }
        }

        // Either no items of the same type were present on the inventory, or the present stacks were already full. Try to slot the item elsewhere
        return StoreAnywhere(GameController.GenerateStackableItem(item.GetName(), item.GetIngredients()));
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

    public static float GetGold() { return instance.gold; }
    public static void ModifyGold(float amount) { instance.gold += amount; 
        InventoryUI.SetGoldUI(instance.gold, amount > 0);
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

    private void InitializeClothing() {
        clothingDictionary = new Dictionary<ClothingItem.ClothingType, Clothing>();

        Dictionary<ClothingItem.ClothingType, GameObject> objects = ClothingController.GenerateClothingObjects();
        clothingDictionary[ClothingItem.ClothingType.Legs] = objects[ClothingItem.ClothingType.Legs].GetComponent<Clothing>();
        clothingDictionary[ClothingItem.ClothingType.Shoes] = objects[ClothingItem.ClothingType.Shoes].GetComponent<Clothing>();
        clothingDictionary[ClothingItem.ClothingType.Torso] = objects[ClothingItem.ClothingType.Torso].GetComponent<Clothing>();
    }

    private void Start()
    {
        Initialize();
    }

    private bool initialized = false;

    private void Initialize()
    {
        if (initialized) { return; }
        initialized = true;

        InitializeDictionary();
        InitializeClothing();

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
        InventoryUI.instance.UpdateSpriteHotbar(slot);

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

    private bool wheelCooldown;

    private IEnumerator WheelCooldown() { wheelCooldown = true; yield return new WaitForSeconds(.075f); wheelCooldown = false; }

    public void Update()
    {
        if (listening)
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel != 0 && !wheelCooldown)
            {
                if (wheel > 0f) { PreviousItem(); } else { NextItem(); }
                StartCoroutine(WheelCooldown());
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
                if (!CraftingController.anyOpen && !UseItem())
                {
                    List<Furniture> list = Cast<Furniture>();

                    Furniture toBePickedUp = null;
                    foreach (Furniture f in list)
                    {
                        if (!f.HasItemsOnTop() && !f.IsBlocked())
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

                        LocationController.GetPathfindingAgent("Tavern").RecalculateBoundaries();
                    }

                }
            }
            

            if (Input.GetMouseButtonDown(1))
            {
                GameObject item = GetCurrentItem();

                Interactuable i = null;

                List<Interactuable> l = Cast<Interactuable>(true);
                List<NPC> npcs = Cast<NPC>(true);

 
                if (l.Count > 0) {
                    // Sort the list. The first element should be the one that has the smallest distance to the mouse.
                    Vector3 worldPosition = GameController.instance.WorldPosition(Input.mousePosition);

                    SortByDistance(l, worldPosition);

                    i = l[0];
                }

                if (i == null)
                {
                    // Rotate held item
                    bool rotated = false;
                    if (item != null && item.TryGetComponent(out Furniture f))
                    {
                        if (f.rotateGameObject != null)
                        {
                            SetCurrentItem(f.rotateGameObject);
                            InventoryUI.instance.UpdateSpriteHotbar(f.rotateGameObject.GetComponent<Item>(), currentItem);
                            rotated = true;
                        }
                    }
                    
                    if (!rotated && npcs.Count > 0)
                    {
                        // Interact with one npc
                        NPC npc = npcs[0];
                        if (Vector2.Distance(npc.gameObject.transform.position, gameObject.transform.position) < 2f)
                        {
                            npc.Interact(item);
                        }
                    }
                }
                else
                {
                    if (Vector2.Distance(gameObject.transform.position, i.GetPosition()) <= i.GetMaxDistance())
                    {
                        if (i.Interact(PlayerMovement.GetInstance())) {
                            // All good
                        } else
                        {
                            // Interaction failed. Maybe possible to interact with an npc
                            if (npcs.Count > 0)
                            {
                                NPC npc = npcs[0];
                                if (Vector2.Distance(npc.gameObject.transform.position, gameObject.transform.position) < 2f) {

                                    npc.Interact(item);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            BookMenuUI.OpenOrCloseMenuPublic();
        }
    }

    private List<Interactuable> SortByDistance(List<Interactuable> l, Vector3 distance)
    {
        Comparison<Interactuable> comparison = (a, b) =>
        {
            GameObject objA = a.GetGameObject();
            GameObject objB = b.GetGameObject();

            float distanceA = Vector3.Distance(objA.transform.position, distance);
            float distanceB = Vector3.Distance(objB.transform.position, distance);

            return distanceA.CompareTo(distanceB);
        };

        l.Sort(comparison);

        return l;
    }

    private List<T> Cast<T>(bool deepSearch = false)
    {
        Vector3 worldPosition = GameController.instance.WorldPosition(Input.mousePosition);

        RaycastHit2D[] rays = Physics2D.RaycastAll(worldPosition, Vector2.zero);

        List<T> fs = new List<T>();

        foreach (var ray in rays)
        {
            if (!deepSearch)
            {
                if (ray.collider.gameObject.TryGetComponent(out T f))
                {
                    fs.Add(f);
                }
            } else
            {
                T[] components = ray.collider.gameObject.GetComponentsInChildren<T>();
                if (components != null)
                {
                    fs.AddRange(components);
                }
            } 
        }

        return fs;
    }

    public bool UseItem()
    {
        if (hotBar[currentItem] != null)
        {
            return hotBar[currentItem].GetComponent<Item>().UseItem();
           // return true;
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
        return new InventoryData(instance.inventory, instance.hotBar, instance.gold);
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
            ModifyGold(d.GetGold());
            InventoryUI.instance.UpdateUI();

            
        }
    }

    public InventoryData ReadData()
    {
        Master m = MasterData.Read();

        if (m == null) return null;
        return m.inventoryData;
    }
}
