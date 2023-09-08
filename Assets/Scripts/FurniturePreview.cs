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

    public void EnablePreview(Furniture item, Vector3 scale) {
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

    public Color defColor;
    public Color wrongColor;

    private IEnumerator PreviewItem(Furniture item)
    {
        spriteRenderer.sprite = item.GetSprite();
        while (true)
        {
            Vector3 worldPosition = GameController.instance.WorldPosition(Input.mousePosition);
            
            MovePreview(GridManager.instance.SnapPosition(worldPosition));
            
            if (item.CanBePlaced(GridManager.instance.GridPosition(worldPosition))) 
            {
                spriteRenderer.color = defColor;
            } else {
                spriteRenderer.color = wrongColor;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
