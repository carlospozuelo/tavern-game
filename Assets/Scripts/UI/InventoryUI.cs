using System.Collections;
using System.Collections.Generic;
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

        if (item != null)
        {
            image.enabled = true;
            image.sprite = item.GetSprite();
        } else
        {
            image.enabled = false;
        }
    }

    public void UpdateUI(int itemHeld)
    {
        slots[currentHeld].GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDef, sizeDef);
        slots[itemHeld].GetComponent<RectTransform>().sizeDelta = new Vector2(sizeHeld, sizeHeld);

        Image image = slots[currentHeld].GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, transDef / 255f);

        image = slots[itemHeld].GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, transHeld / 255f);

        currentHeld = itemHeld;
    }
}