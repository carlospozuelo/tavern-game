using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 6f;
    public Rigidbody2D rb;

    public Animator animator;

    [SerializeField]
    private bool sitting = false;

    private static PlayerMovement instance;
    private Bench bench;

    public static bool IsSitting()
    {
        return instance.sitting;
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

    private void Start()
    {
        ClothingController.SetAnimator(animator);
        ClothingController.UpdateColorsReverse(); 
    }

    public static void Sit(Vector2 position, Bench bench)
    {
        instance.transform.position = new Vector3(position.x, position.y, instance.transform.position.z);
        instance.animator.SetBool("Sitting", true);
        instance.animator.SetFloat("AnimMoveX", bench.direction.x);
        instance.animator.SetFloat("AnimMoveY", bench.direction.y);
        instance.sitting = true;
        instance.bench = bench;
        instance.Stop();
    }

    private void GetUp(float h, float v)
    {
        if (bench.GetUp(gameObject, h, v))
        {
            bench = null;
            sitting = false;
            animator.SetBool("Sitting", false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            if (!sitting)
            {

                Vector2 normalized = new Vector2(h, v).normalized;

                animator.SetFloat("MovementMagnitude", normalized.magnitude);
                animator.SetFloat("AnimMoveX", h);
                animator.SetFloat("AnimMoveY", v);

                rb.velocity = normalized * speed;
            } else
            {
                GetUp(h, v);
            }
        } else
        {
            Stop();
        }
    }

    private void Stop()
    {
        rb.velocity = Vector2.zero;
        animator.SetFloat("MovementMagnitude", 0);
    }
}
