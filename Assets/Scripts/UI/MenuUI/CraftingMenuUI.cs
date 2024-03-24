using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMenuUI : MenuUI
{
    [SerializeField]
    private DragDrop[] crafting;

    public override void UpdateImage()
    {
        base.UpdateImage();
        UpdateImage(crafting);
    }
}
