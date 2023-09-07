using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurniturePreview : MonoBehaviour
{

    public static FurniturePreview instance;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void MovePreview(Vector2 position)
    {
        transform.position = position;
    }

    public void EnablePreview(Item item, Vector3 scale) {
        spriteRenderer.enabled = true;
        transform.localScale = scale;
        previewCoroutine = StartCoroutine(PreviewItem(item));
    }

    private Coroutine previewCoroutine;
    public void DisablePreview()
    {
        spriteRenderer.enabled = false;
        if (previewCoroutine != null) { StopCoroutine(previewCoroutine); }
    }

    private IEnumerator PreviewItem(Item item)
    {
        spriteRenderer.sprite = item.GetSprite();
        while (true)
        {
            MovePreview(GridManager.instance.SnapPosition(GameController.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition)));
            yield return new WaitForEndOfFrame();
        }
    }
}
