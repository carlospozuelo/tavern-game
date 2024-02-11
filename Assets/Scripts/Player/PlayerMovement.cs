using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : CharacterAbstract
{

    public float speed = 6f;

    public static Animator GetAnimator() { return instance.animator; }

    private static PlayerMovement instance;
    public static PlayerMovement GetInstance() { return instance; }

    [SerializeField]
    private Transform up, down, left, right;

    public static Collider2D[] GetCollidersAtBox(Vector2 direction)
    {
        if (direction == Vector2.right) { return GetCollidersAtBox(instance.right); }
        if (direction == Vector2.up) { return GetCollidersAtBox(instance.up); }
        if (direction == Vector2.left) { return GetCollidersAtBox(instance.left); }
        if (direction == Vector2.down) { return GetCollidersAtBox(instance.down); }

        return null;
    }

    public static void StopMovement(float duration)
    {
        instance.StopMovementPriv(duration);
    }


    private Coroutine stopMovementCoroutine;

    private void StopMovementPriv(float duration)
    {
        if (stopMovementCoroutine != null) { StopCoroutine(stopMovementCoroutine); }
        StartCoroutine(StopMovementCorr(duration));
    }

    private IEnumerator StopMovementCorr(float duration)
    {
        Stop();
        canMove = false;

        yield return new WaitForSeconds(duration);

        canMove = true;
    }


    private static Collider2D[] GetCollidersAtBox(Transform t)
    {
        return Physics2D.OverlapBoxAll(t.position, new Vector2(1f, 1f), 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;


        Gizmos.DrawWireCube(up.position, new Vector3(1f, 1f, 1f));
        Gizmos.DrawWireCube(down.position, new Vector3(1f, 1f, 1f));
        Gizmos.DrawWireCube(left.position, new Vector3(1f, 1f, 1f));
        Gizmos.DrawWireCube(right.position, new Vector3(1f, 1f, 1f));
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
        Initialize();
    }

    public static Vector3 GetPosition()
    {
        return instance.rb.position;
    }

    public static void LookAt(Vector2 direction)
    {
        instance.animator.SetFloat("AnimMoveX" ,direction.x);
        instance.animator.SetFloat("AnimMoveY", direction.y);
    }

    public static void ToggleColliders(bool value)
    {
        instance.ToggleCollidersPrivate(value);
    }
    
    public static void ToggleMasking(SpriteMaskInteraction maskInteraction)
    {
        instance.ToggleMaskingPrivate(maskInteraction);
    }

    
    // Update is called once per frame
    private bool canMove = true;
    void Update()
    {
        if (!canMove) { return; }

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
}
