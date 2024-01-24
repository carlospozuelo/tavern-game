using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour, Item
{


    [SerializeField]
    private GameObject originalPrefab;

    [SerializeField]
    private SpriteRenderer s;

    [SerializeField]
    private int damage = 1;

    void Item.CancelSelectItem()
    {
    }

    string Item.GetName()
    {
        return originalPrefab.name;
    }

    GameObject Item.GetOriginalPrefab()
    {
        return originalPrefab;
    }

    Sprite Item.GetSprite()
    {
        return s.sprite;
    }

    void Item.SelectItem()
    {
        // Trigger player animation
    }

    public Vector2 GetRelativePosition(Vector2 a, Vector2 b)
    {
        Vector2 direction = a - b;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal comparison
            if (direction.x > 0)
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }
        else
        {
            // Vertical comparison
            if (direction.y > 0)
            {
                return Vector2.up;
            }
            else
            {
                return Vector2.down;
            }
        }
    }

    public float cooldown = .5f;

    bool Item.UseItem()
    {
        if (CoroutineHandler.IsItemCooldown()) { return true; }

        CoroutineHandler.ItemCooldown(cooldown);

        Vector2 pos = GameController.instance.WorldMousePosition();
        Vector2 playerPos = PlayerMovement.GetPosition();

        Vector2 relativePos = GetRelativePosition(pos, playerPos);

        PlayerMovement.LookAt(relativePos);

        Collider2D[] box = PlayerMovement.GetCollidersAtBox(relativePos);

        if (box != null)
        {
            foreach (Collider2D collider in box)
            {
                if (collider.TryGetComponent(out Tree tree))
                {
                    tree.Hit(damage);
                }
            }
        }

        PlayerMovement.GetAnimator().SetTrigger("Bonk");
        PlayerMovement.StopMovement(.25f);

        return true;

    }
}
