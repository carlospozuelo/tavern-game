using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
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

    private bool draggable;


    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (DraggableIcon.GetDraggable() != null && DraggableIcon.GetDraggable().draggable)
            {
                targetImage.sprite = DraggableIcon.GetSprite();
                targetImage.enabled = true;
                draggable = true;
                // Target is inventory
                if (isInventory)
                {
                    PlayerInventory.instance.SetInventory(index, DraggableIcon.GetDraggable().GetItem());
                }
                else if (isHotbar)
                {
                    PlayerInventory.instance.SetHotBar(index, DraggableIcon.GetDraggable().GetItem());
                }

                
                DraggableIcon.GetDraggable().DestroyItem();
                DraggableIcon.HideImage();
            }
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

        if (isInventory)
        {
            PlayerInventory.instance.SetInventory(index, null);
        } else if (isHotbar)
        {
            PlayerInventory.instance.SetHotBar(index, null);
        }
    }

    private void Start()
    { 
        canvas = BookMenuUI.GetCanvas();
        if (target == null) { target = gameObject; }
        if (targetImage == null) { targetImage = target.GetComponent<Image>();  }
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (draggable)
        {
            targetImage.enabled = false;
            DraggableIcon.DisplayImage(targetImage.sprite, rectTransform.position, this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggable)
        {
            DraggableIcon.MoveImage(eventData.delta / canvas.scaleFactor);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggable)
        {
            targetImage.enabled = true;
            DraggableIcon.HideImage();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
