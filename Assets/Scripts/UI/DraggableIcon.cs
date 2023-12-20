using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableIcon : MonoBehaviour
{
    private static DraggableIcon instance;
    private RectTransform position;

    private CanvasGroup c;

    [SerializeField]
    private Image image;

    private DragDrop draggable;

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


    public static void DisplayImage(Sprite sprite, Vector2 position, DragDrop d)
    {
        instance.image.sprite = sprite;
        instance.image.enabled = true;
        instance.position.position = position;
        instance.c.blocksRaycasts = false;
        instance.draggable = d;
    }

    public static void HideImage()
    {
        instance.image.enabled = false;
        instance.c.blocksRaycasts = true;
        instance.draggable = null;
    }

    public static void MoveImage(Vector2 movement)
    {
        instance.position.anchoredPosition += movement;
    }
}
