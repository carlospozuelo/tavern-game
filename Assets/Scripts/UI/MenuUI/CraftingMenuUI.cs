using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMenuUI : MenuUI
{
    [SerializeField]
    private DragDrop[] crafting;

    private static CraftingMenuUI instance;

    [SerializeField]
    private RectTransform progressArrow;

    [SerializeField]
    private float progressMaxWidth = 40f;

    private Coroutine arrowCorroutine;

    public static void SetProgress (float totalTime, float elapsedTime)
    {
        if (instance != null)
        {
           instance.progressArrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (elapsedTime / totalTime) * instance.progressMaxWidth);
        }
    }

    private void OnEnable()
    {
        if (instance != null)
        {
            Debug.LogWarning("Crafting menu ui is not null. There should only be one crafting menu active at all times");
        }

        instance = this;
        instance.progressArrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);

    }

    private void OnDisable()
    {
        if (instance != null && instance == this)
        {
            if (arrowCorroutine != null)
            {
                StopCoroutine(arrowCorroutine);
            }
            instance = null;
        }
    }

    public override void UpdateImage()
    {
        base.UpdateImage();
        UpdateImage(crafting);
    }
}
