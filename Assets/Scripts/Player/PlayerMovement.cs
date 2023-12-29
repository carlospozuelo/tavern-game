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

    private Collider2D[] colliders;

    private SpriteRenderer[] renderers;

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
        colliders = GetComponentsInChildren<Collider2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public static void Sit(Vector2 position, Bench bench)
    {
        ToggleColliders(false);

        instance.transform.position = new Vector3(position.x, position.y, instance.transform.position.z);
        instance.animator.SetBool("Sitting", true);
        instance.animator.SetFloat("AnimMoveX", bench.direction.x);
        instance.animator.SetFloat("AnimMoveY", bench.direction.y);
        instance.sitting = true;
        instance.bench = bench;
        instance.Stop();
        ToggleMasking(SpriteMaskInteraction.VisibleOutsideMask);

        SpriteMask[] masks = bench.GetFurniture().gameObject.GetComponentsInChildren<SpriteMask>();
        foreach (var mask in masks) { mask.enabled = true; }
    }

    public static void ToggleColliders(bool value)
    {
        foreach (Collider2D c in instance.colliders)
        {
            c.enabled = value;
        }
    }

    public static void ToggleMasking(SpriteMaskInteraction maskInteraction)
    {
        foreach (SpriteRenderer s in instance.renderers)
        {
            s.maskInteraction = maskInteraction;
        }
    }

    private void GetUp(float h, float v)
    {
        if (bench.GetUp(gameObject, h, v))
        {
            SpriteMask[] masks = bench.GetFurniture().gameObject.GetComponentsInChildren<SpriteMask>();
            foreach (var mask in masks) { mask.enabled = false; }

            bench = null;
            sitting = false;
            animator.SetBool("Sitting", false);

            ToggleColliders(true);
            ToggleMasking(SpriteMaskInteraction.None);
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
                InventoryUI.ToggleGold(.2f);
            }
            else
            {
                GetUp(h, v);
            }
        } else
        {
            InventoryUI.ToggleGold(1f);
            Stop();
        }

    }

    private void Stop()
    {
        rb.velocity = Vector2.zero;
        animator.SetFloat("MovementMagnitude", 0);
    }
}
