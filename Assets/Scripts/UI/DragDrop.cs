using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler
{
    private RectTransform rectTransform;

    private Canvas canvas;

    [SerializeField]
    private GameObject target;

    // Slot
    [SerializeField]
    private bool isInventory, isHotbar;
    [SerializeField]
    private int index;
    [SerializeField]
    private Image targetImage;
    [SerializeField]
    private bool draggable;


    private void Start()
    {
        canvas = BookMenuUI.GetCanvas();
        if (target == null) { target = gameObject; }
        if (targetImage == null) { targetImage = target.GetComponent<Image>(); }
        rectTransform = target.GetComponent<RectTransform>();

        if (isInventory)
        {
            draggable = PlayerInventory.instance.inventory[index] != null;
        }
        else if (isHotbar)
        {
            draggable = PlayerInventory.instance.hotBar[index] != null;
        }
    }

    public GameObject GetItem()
    {
        if (isInventory)
        {
            return PlayerInventory.instance.inventory[index];
        }

        if (isHotbar)
        {
            return PlayerInventory.instance.hotBar[index];
        }

        return null;
    }

    public void UpdateImage()
    {
        GameObject g = GetItem();

        if (g != null)
        {
            targetImage.sprite = g.GetComponent<Item>().GetSprite();
            targetImage.enabled = true;
        } else
        {
            targetImage.enabled = false;
        }
    }

    public void DestroyItem()
    {
        draggable = false;
        targetImage.enabled = false;

        SlotItem(null);
    }

    private void SlotItem(GameObject item)
    {
        if (isInventory)
        {
            PlayerInventory.instance.SetInventory(index, item);//DraggableIcon.GetDraggable().GetItem());
        }
        else if (isHotbar)
        {
            PlayerInventory.instance.SetHotBar(index, item);//DraggableIcon.GetDraggable().GetItem());
        }

        if (item != null)
        {
            draggable = true;
        }

        UpdateImage();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!BookMenuUI.IsOpen()) { return;  }
        if (GetItem() != null && DraggableIcon.GetItemHeld() != null)
        {
            // Swap
        }
        else
        {
            if (draggable)
            {
                // Stacks would be implemented here
                DraggableIcon.DisplayImage(targetImage.sprite, rectTransform.position, this, GetItem());
                // Remove item from the inventory
                DestroyItem();
            }
            else
            {
                GameObject itemHeld = DraggableIcon.GetItemHeld();
                if (itemHeld != null)
                {
                    SlotItem(itemHeld);
                    DraggableIcon.HideImage();
                }
            }
        }
    }
}
