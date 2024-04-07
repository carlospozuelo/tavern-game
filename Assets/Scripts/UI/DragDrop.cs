using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
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
    private bool isInventory, isHotbar, isSlottable;
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

    [SerializeField]
    private Ingredient.IngredientType acceptedTypes;


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
        } else if (isSlottable)
        {
            // Slot on the current open menu
            draggable = CraftingController.GetOpenIndex(index) != null;
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

        if (isSlottable)
        {
            var item = CraftingController.GetOpenIndex(index);

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
            if (stackText != null) { stackText.text = ""; }
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
            if (stackText != null)
            {
                stackText.text = "";
            }
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
        } else if (isSlottable)
        {
            CraftingController.SetOpenIndex(index, item);
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

        if (!CraftingController.anyOpen) { return; }

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
            if (!TypeMatches(DraggableIcon.GetItemHeld())) { return; }



            // If the item is stackable AND it's of the same type as the draggable icon item
            if (temp.TryGetComponent(out StackableItem stackableItem))
            {
                if (!stackableItem.IsFull()) { 
                    if (DraggableIcon.GetItemHeld().TryGetComponent(out StackableItem draggableItem))
                    {
                        if (stackableItem.GetName().Equals(draggableItem.GetName()))
                        {
                            // Left click
                            if (eventData.button == PointerEventData.InputButton.Left)
                            {
                                // Add the stacks together.
                                int leftover = stackableItem.IncrementStacks(draggableItem.GetStacks());

                                if (leftover == 0)
                                {
                                    // Destroy the draggable
                                    Destroy(draggableItem.gameObject);
                                    DraggableIcon.HideImage(false);
                                    return;
                                }
                                else
                                {
                                    draggableItem.SetStacks(leftover);
                                    DraggableIcon.UpdateStacks(draggableItem.GetStacks());
                                    return;
                                }
                            }

                            // Right click
                            if (eventData.button == PointerEventData.InputButton.Right)
                            {
                                // Increment the stack by one
                                stackableItem.IncrementStacks();
                                draggableItem.SetStacks(draggableItem.GetStacks() - 1);

                                if (draggableItem.GetStacks() == 0) {
                                    Destroy(draggableItem.gameObject);
                                    DraggableIcon.HideImage(false);
                                } else
                                {
                                    DraggableIcon.UpdateStacks(draggableItem.GetStacks());
                                }

                                return;
                            }
                        }
                    }
                }
            }

            Sprite s = targetImage.sprite;

            SlotItem(DraggableIcon.GetItemHeld());

            DraggableIcon.DisplayImage(s, this, temp);
        }
        else
        {

            if (draggable)
            {
                // Pick up
                bool canBePickedUp = true;
                if (isForClothing) {
                    canBePickedUp = type != ClothingItem.ClothingType.Legs && type != ClothingItem.ClothingType.Torso;
                    // Torso and leg clothing can't be removed
                }

                if (canBePickedUp)
                {

                    GameObject item = GetItem();

                    if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        if (item.TryGetComponent(out StackableItem stackable)) {
                            // Pick half of the stacks
                            bool aux = stackable.GetStacks() % 2 == 0;
                            int stacks1 =  aux ? stackable.GetStacks() / 2 : (stackable.GetStacks() + 1) / 2;
                            int stacks2 =  aux ? stacks1 : stacks1 - 1;

                            DraggableIcon.DisplayImage(targetImage.sprite, this, GameController.GenerateStackableItem(stackable.GetName(), stacks1));

                            stackable.SetStacks(stacks2);
                            if (stackable.GetStacks() <= 0)
                            {
                                DestroyItem();
                            }

                            return;
                        }
                    }

                    DraggableIcon.DisplayImage(targetImage.sprite, this, item);
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
                // Slot

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

                    // Check, if it is a resource slot, if the required type matches
                    if (!TypeMatches(itemHeld)) { return; }

                    if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        if (itemHeld.TryGetComponent(out StackableItem stackable))
                        {
                            // Stack just one
                            SlotItem(GameController.GenerateStackableItem(stackable.GetName()));
                            stackable.SetStacks(stackable.GetStacks() - 1);

                            if (stackable.GetStacks() == 0)
                            {
                                Destroy(stackable.gameObject);
                                DraggableIcon.HideImage(false);
                            }
                            else
                            {
                                DraggableIcon.UpdateStacks(stackable.GetStacks());
                            }

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

    private bool TypeMatches(GameObject itemHeld)
    {
        if (isSlottable)
        {
            // Can only be slotted if the ingredient matches the required type
            if (itemHeld.TryGetComponent(out StackableItem stackable))
            {
                if (stackable.GetIngredient().HasAnyType(acceptedTypes))
                {
                    return true;
                }
            }
            return false;
        }

        return true;
    }
}
