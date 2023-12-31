using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private Item item;
    [SerializeField]
    private float detectionRadius = 2.5f, baseSpeed = 0.5f;

    private bool justSpawned = true;

    public void Initialize(Item item)
    {
        name = item.GetName();

        this.item = item;
        spriteRenderer.sprite = item.GetSprite();

        float width = item.GetSprite().rect.width;
        float height = item.GetSprite().rect.height;
        justSpawned = true;
        spriteRenderer.transform.localScale = new Vector3(32 / width, 32 / height);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void Update()
    {
        if (!justSpawned)
        {
            Vector2 playerPosition = PlayerMovement.GetPosition();

            float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), playerPosition);
            if (distance <= 0.2f)
            {
                Debug.Log("Store in inventory");
                if (PlayerInventory.StoreAnywhere(item))
                {
                    Destroy(gameObject);
                }
            }
            else if (distance < detectionRadius)
            {
                // Move towards player, increasingly faster until distance is 0
                Vector2 direction = (playerPosition - new Vector2(transform.position.x, transform.position.y)).normalized;
                float speed = Mathf.Max(baseSpeed * (1f - distance / detectionRadius), 0f);

                transform.Translate(direction * speed * Time.deltaTime);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            justSpawned = false;
        }
    }
}
