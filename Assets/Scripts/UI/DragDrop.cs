using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    private bool draggable, isForClothing;

    [SerializeField]
    private ClothingItem.ClothingType type;

    [SerializeField]
    private TMPro.TextMeshProUGUI stackText;


    private void Start()
    {
        canvas = BookMenuUI.GetCanvas();
        if (target == null) { target = gameObject; }
        if (targetImage == null) { targetImage = target.GetComponent<Image>(); }
        rectTransform = target.GetComponent<RectTransform>();

        SetDraggable();
    }

    private void SetDraggable()
    {
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

        if (isForClothing)
        {
            var item = PlayerInventory.GetWornItem(type);

            if (item != null) return item.gameObject;
        }

        return null;
    }

    public void UpdateStacks(Item i)
    {
        if (stackText != null)
        {
            if (i is StackableItem)
            {
                // Is stackable. Set stacks
                stackText.text = ((StackableItem) i).GetStacks() + "";
            }
            else
            {
                stackText.text = "";
            }
        }
    }

    public void UpdateImage()
    {
        GameObject g = GetItem();
        if (g != null)
        {
            Item i = g.GetComponent<Item>();
            UpdateStacks(i);

            targetImage.sprite = i.GetSprite();
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

        if (item != null && item.TryGetComponent(out StackableItem stack))
        {
            // Is stackable. Set stacks
            stackText.text = stack.GetStacks() + "";
        } else
        {
            stackText.text = "";
        }

        if (isForClothing)
        {
            item.GetComponent<Clothing>().Wear();
        }

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
        SetDraggable();

        if (!BookMenuUI.IsOpen()) { return; }
        //if (!isForClothing)
        //{
        if (GetItem() != null && DraggableIcon.GetItemHeld() != null)
        {
            // Swap

            if (isForClothing)
            {
                // Can only be swapped by a clothing item of the same type
                if (!DraggableIcon.GetItemHeld().TryGetComponent(out Clothing c) || c.GetClothingItem().type != type)
                {
                    return;
                }
            }

            GameObject temp = GetItem();
            Sprite s = targetImage.sprite;

            SlotItem(DraggableIcon.GetItemHeld());

            DraggableIcon.DisplayImage(s, this, temp);
        }
        else
        {
            if (draggable)
            {
                // Stacks would be implemented here
                bool canBePickedUp = true;
                if (isForClothing) {
                    canBePickedUp = type != ClothingItem.ClothingType.Legs && type != ClothingItem.ClothingType.Torso;
                    // Torso and leg clothing can't be removed
                }

                if (canBePickedUp)
                {
                    DraggableIcon.DisplayImage(targetImage.sprite, this, GetItem());
                    // Remove item from the inventory
                    DestroyItem();

                    if (isForClothing)
                    {
                        // TODO: Make player stop wearing clothes (shoes)
                        // ClothingController.Wear()
                    }
                }
            }
            else
            {
                GameObject itemHeld = DraggableIcon.GetItemHeld();
                if (itemHeld != null)
                {
                    if (isForClothing)
                    {
                        // Can only be swapped by a clothing item of the same type
                        if (!DraggableIcon.GetItemHeld().TryGetComponent(out Clothing c) || c.GetClothingItem().type != type)
                        {
                            return;
                        }
                    }

                    SlotItem(itemHeld);
                    DraggableIcon.HideImage(false);
                }
            }
        }
        //}
    }
}
