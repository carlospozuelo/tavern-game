using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

    public static InventoryUI instance;

    public GameObject[] slots;
    private int currentHeld = 0;

    public int sizeHeld = 50;
    public int sizeDef = 40;

    public int transHeld = 200;
    public int transDef = 125;

    [SerializeField]
    private CanvasGroup goldCanvasGroup;

    [SerializeField]
    private TextMeshProUGUI goldCount;

    public static void SetGoldUI(int value)
    {
        instance.goldCount.text = "" + value;
        // Play animation or sound whenever gaining / spending money -> This would be handled here
    }

    public static void ToggleGold(float trans)
    {
        instance.goldCanvasGroup.alpha = trans;
    }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void UpdateSpriteHotbar(Item item, int slot)
    {
        Image image = slots[slot].transform.GetChild(0).GetComponent<Image>();

        if (item is StackableItem)
        {
            DragDrop d = slots[slot].GetComponent<DragDrop>();
            d.UpdateStacks(item);
        }

        if (item != null)
        {
            image.enabled = true;
            image.sprite = item.GetSprite();
        } else
        {
            image.enabled = false;
        }
    }

    public void UpdateSpriteHotbar(int slot)
    {
        GameObject item = PlayerInventory.instance.hotBar[slot];

        if (item != null) { UpdateSpriteHotbar(item.GetComponent<Item>(), slot); }
    }

    public static void SelectAllItems()
    {
        foreach (var slot in instance.slots)
        {
            slot.GetComponent<RectTransform>().sizeDelta = new Vector2(instance.sizeHeld, instance.sizeHeld);
            Image image = slot.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, instance.transHeld / 255f);
        }
    }

    public static void DeselectAllItems()
    {
        foreach (var slot in instance.slots)
        {
            slot.GetComponent<RectTransform>().sizeDelta = new Vector2(instance.sizeDef, instance.sizeDef);
            Image image = slot.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, instance.transDef / 255f);
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            UpdateSpriteHotbar(i);
        }

        CraftingController.UpdateAll();
        //BookMenuUI.UpdateImagePublic();
    }

    public void UpdateUI(int itemHeld)
    {
        if (BookMenuUI.IsOpen()) { return; }
        UpdateSpriteHotbar(itemHeld);

        slots[currentHeld].GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDef, sizeDef);
        slots[itemHeld].GetComponent<RectTransform>().sizeDelta = new Vector2(sizeHeld, sizeHeld);

        Image image = slots[currentHeld].GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, transDef / 255f);

        image = slots[itemHeld].GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, transHeld / 255f);

        currentHeld = itemHeld;
    }
}