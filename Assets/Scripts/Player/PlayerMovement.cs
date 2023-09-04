using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 6f;
    public Rigidbody2D rb;

    public Animator animator;


    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            

            Vector2 normalized = new Vector2(h, v).normalized;

            animator.SetFloat("MovementMagnitude", normalized.magnitude);
            animator.SetFloat("AnimMoveX", h);
            animator.SetFloat("AnimMoveY", v);

            rb.velocity = normalized * speed;
        } else
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("MovementMagnitude", 0);
        }
    }
}
