using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMenuUI : MenuUI
{
    private static BookMenuUI instance;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private DragDrop[] clothingSlots;

    public static Canvas GetCanvas()
    {
        return instance.canvas;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }

    public static void OpenOrCloseMenuPublic()
    {
        instance.OpenOrCloseMenu();
    }

    public static void UpdateImagePublic ()
    {
        instance.UpdateImage();
    }

    public override void UpdateImage()
    {
        base.UpdateImage();
        instance.UpdateImage(instance.clothingSlots);
    }
    public static bool IsOpen() { return instance.isOpen; }

}
