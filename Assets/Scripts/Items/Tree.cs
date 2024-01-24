using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{

    [SerializeField]
    private int minHealth = 8, maxHealth = 10, currentHealth, minWood = 3, maxWood = 6, currentWood;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Vector2 anchor, anchorR;

    [SerializeField]
    private GameObject woodItem;
    private bool destroying = false;
    private Vector2 GetAnchor() { return new Vector2(transform.position.x, transform.position.y) + anchor ; }

    private Vector2 GetAnchorR() { return new Vector2(transform.position.x, transform.position.y) + anchorR; }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetAnchor(), 1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GetAnchorR(), 1f);
    }

    private void Start()
    {
        currentHealth = Random.Range(minHealth, maxHealth);
        currentWood = Random.Range(minWood, maxWood);
    }

    public void SpawnLoot()
    {
        Vector2 aux = (PlayerMovement.GetPosition().x < transform.position.x) ? GetAnchorR() : GetAnchor();

        for (int i = 0; i < currentWood; i++)
        {
            GameController.DropItem(woodItem.GetComponent<StackableItem>(), aux + new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized, false);
        }
    }

    public void Hit(int damage) { 
        if (destroying) { return; }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Vector3 p = PlayerMovement.GetPosition();

            if (p.x < transform.position.x) { 
                animator.SetTrigger("Fall reverse");
            } else
            {
                animator.SetTrigger("Fall");
            }

            destroying = true;
            Destroy(gameObject, 5f);
        }
        else
        {
            animator.SetTrigger("Shake");
        }
    }
}
