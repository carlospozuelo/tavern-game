using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Backpanel : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {   
        if (DraggableIcon.GetItemHeld() == null)
        {
            CraftingController.CloseAllMenus(1f);
        } else
        {
            DraggableIcon.HideImage(true);

        }
    }
}
