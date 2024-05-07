using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that PlayerMovement and NPC derive from
public abstract class CharacterAbstract : MonoBehaviour
{
    protected Bench bench;

    [SerializeField]
    protected bool sitting = false;

    protected Collider2D[] colliders;

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected Rigidbody2D rb;

    protected SpriteRenderer[] renderers;

    private void OnEnable()
    {
        if (sitting)
        {
            animator.SetBool("Sitting", true);
            animator.SetFloat("AnimMoveX", bench.direction.x);
            animator.SetFloat("AnimMoveY", bench.direction.y);
        }
    }

    protected void Stop()
    {
        rb.velocity = Vector2.zero;
        animator.SetFloat("MovementMagnitude", 0);
    }

    protected void ToggleMaskingPrivate(SpriteMaskInteraction maskInteraction)
    {
        foreach (SpriteRenderer s in renderers)
        {
            s.maskInteraction = maskInteraction;
        }
    }

    public bool IsSitting()
    {
        return sitting;
    }

    protected bool initialized = false;
    protected void Initialize()
    {
        if (initialized) return;

        colliders = GetComponentsInChildren<Collider2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        initialized = true;
    }

    private void Start()
    {
        Initialize();
    }

    public void Sit(Vector2 position, Bench bench)
    {
        ToggleCollidersPrivate(false);

        transform.position = new Vector3(position.x, position.y, transform.position.z);
        animator.SetBool("Sitting", true);
        animator.SetFloat("AnimMoveX", bench.direction.x);
        animator.SetFloat("AnimMoveY", bench.direction.y);
        sitting = true;
        this.bench = bench;
        Stop();
        ToggleMaskingPrivate(SpriteMaskInteraction.VisibleOutsideMask);

        SpriteMask[] masks = bench.GetFurniture().gameObject.GetComponentsInChildren<SpriteMask>();
        foreach (var mask in masks) { mask.enabled = true; }

        NPCController.RemoveBenchForNPC(bench);
    }

    protected void ToggleCollidersPrivate(bool value)
    {
        foreach (Collider2D c in colliders)
        {
            if (!c.isTrigger)
            {
                c.enabled = value;
            }
        }
    }

    protected void GetUp(float h, float v)
    {
        if (bench.GetUp(gameObject, h, v))
        {
            SpriteMask[] masks = bench.GetFurniture().gameObject.GetComponentsInChildren<SpriteMask>();
            foreach (var mask in masks) { mask.enabled = false; }

            bench = null;
            sitting = false;
            animator.SetBool("Sitting", false);

            ToggleCollidersPrivate(true);
            ToggleMaskingPrivate(SpriteMaskInteraction.None);
        }
    }
}
