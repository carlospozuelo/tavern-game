using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableIcon : MonoBehaviour, IPointerDownHandler
{
    private static DraggableIcon instance;
    private RectTransform position;

    private CanvasGroup c;

    [SerializeField]
    private Image image;

    private DragDrop draggable;

    [SerializeField]
    private TMPro.TextMeshProUGUI stackText;

    public static DragDrop GetDraggable()
    {
        return instance.draggable;
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
            position = GetComponent<RectTransform>();
            c = GetComponent<CanvasGroup>();
        }
    }

    public static Sprite GetSprite()
    {
        return instance.image.sprite;
    }

    [SerializeField]
    private GameObject itemHeld;

    public static GameObject GetItemHeld()
    {
        return instance.itemHeld;
    }

    public static void DisplayImage(Sprite sprite, DragDrop d, GameObject item)
    {
        instance.image.sprite = sprite;
        instance.image.enabled = true;
        instance.position.position = Input.mousePosition;//position;
        instance.c.blocksRaycasts = true;
        instance.draggable = d;
        instance.itemHeld = item;

        if (item != null)
        {
            if (item.TryGetComponent(out StackableItem stack))
            {
                instance.stackText.text = stack.GetStacks() + "";
            } else
            {
                instance.stackText.text = "";
            }
        }

        instance.MoveImage();
    }

    public static void HideImage(bool dropsItem = true)
    {
        instance.image.enabled = false;
        instance.c.blocksRaycasts = true;
        instance.draggable = null;

        instance.stackText.text = "";

        instance.Stop(dropsItem);
    }

    private void Stop(bool dropsItem)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            // TODO: Drop held item to the ground
            if (dropsItem && itemHeld != null)
            {
                GameController.DropItem(itemHeld.GetComponent<Item>());
            }
            itemHeld = null;
        }
    }

    private Coroutine coroutine;
    private Vector3 startingPoint;

    private void MoveImage()
    {
        //Stop();

        coroutine = StartCoroutine(MoveImageCoroutine());

    }



    private IEnumerator MoveImageCoroutine()
    {
        startingPoint = Input.mousePosition;
        while (true)
        {
            Vector3 newPoint = Input.mousePosition;
            MoveImage((newPoint - startingPoint) / BookMenuUI.GetCanvas().scaleFactor);
            startingPoint = newPoint;
            yield return new WaitForEndOfFrame();
        }
    }

    private static void MoveImage(Vector2 movement)
    {
        instance.position.anchoredPosition += movement;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);


        foreach (RaycastResult result in raycastResults) {
            if (result.gameObject != gameObject)
            {
                ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerDownHandler);
            }
        }

        if (raycastResults.Count <= 1)
        {
            HideImage();
        }
    }
}
