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
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public void EnablePreview(Furniture item, Vector3 scale) {
        spriteRenderer.enabled = true;
        transform.localScale = scale;
        previewCoroutine = StartCoroutine(PreviewItem(item));
    }

    public void EnablePreview()
    {
        spriteRenderer.enabled = true;
    }

    public void HidePreview()
    {
        spriteRenderer.enabled = false;
    }

    private Coroutine previewCoroutine;
    public void DisablePreview()
    {
        spriteRenderer.enabled = false;
        spriteRenderer.sprite = null;
        if (previewCoroutine != null) { StopCoroutine(previewCoroutine); }
    }

    public Color defColor;
    public Color wrongColor;

    private IEnumerator PreviewItem(Furniture item)
    {
        spriteRenderer.sprite = item.GetSprite();
        transform.position = new Vector3(transform.position.x, transform.position.y, item.gameObject.transform.position.z);
        while (true)
        {
            Vector3 worldPosition = GameController.instance.WorldPosition(Input.mousePosition);

            // Try to place the item in a table
            if (item.CanBePlacedOnATable(worldPosition, out Vector3 tablePosition, out Furniture table))
            {
                MovePreview(tablePosition);
                spriteRenderer.color = defColor;
            }
            else
            {
                MovePreview(GridManager.instance.SnapPosition(worldPosition));

                if (item.CanBePlaced(GridManager.instance.GridPosition(worldPosition)))
                {
                    spriteRenderer.color = defColor;
                }
                else
                {
                    spriteRenderer.color = wrongColor;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
