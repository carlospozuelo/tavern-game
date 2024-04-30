using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : CharacterAbstract
{
    [SerializeField]
    private SpriteRenderer body, arms, face, torso, hair, legs, shoes;
    private Material torsoMaterial, hairMaterial, legsMaterial, faceMaterial, shoeMaterial;

    private Dictionary<ClothingItem.ClothingType, ClothingItem> current;
    private string location;

    public void SetLocation(string location) { this.location = location; }
    public string GetLocation() { return location; }

    [SerializeField]
    private float speed = 2;

    [SerializeField]
    private GameObject textBubble;

    [SerializeField]
    private Image desireImage;

    [SerializeField]
    private TextMeshProUGUI desireText;

    [SerializeField]
    private Animator bubbleAnimator;

    

    public void Initialize(Dictionary<ClothingItem.ClothingType, ClothingItem> clothes) {

        torsoMaterial = torso.material;
        hairMaterial = hair.material;
        legsMaterial = legs.material;
        faceMaterial = face.material;
        shoeMaterial = shoes.material;

        current = clothes;

        UpdateColors(torsoMaterial, clothes[ClothingItem.ClothingType.Torso]);
        UpdateColors(hairMaterial, clothes[ClothingItem.ClothingType.Hair]);
        UpdateColors(legsMaterial, clothes[ClothingItem.ClothingType.Legs]);
        UpdateColors(faceMaterial, clothes[ClothingItem.ClothingType.Faces]);
        UpdateColors(shoeMaterial, clothes[ClothingItem.ClothingType.Shoes]);


        GenerateAOC();
        Initialize();
        // Default: Spwan in tavern- so assign a tavern task.
        StartCoroutine(Exist());
    }

    private IEnumerator Exist()
    {
        bool loggedInactivity = false;
        while (true)
        {
            // Select a task randomly (for now just go to a bench)
            Bench bench = NPCController.PopRandomBench();
            if (bench != null) {
                loggedInactivity = false;
                yield return WalkTowardsBench(bench);
                if (!sitting)
                {
                    // Something went wrong. Reinstall the bench
                    NPCController.AddBenchForNPC(bench);
                    Stop();
                }
            }
            else
            {
                // Nothing else to do
                Stop();
                if (!loggedInactivity)
                {
                    Debug.LogWarning("There's nothing for the npc to do! Wander around?");
                    loggedInactivity = true;
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator WalkTowardsBench(Bench bench)
    {
        Vector3 benchPosition = bench.GetPosition();
        Pathfinding pathfinding = LocationController.GetPathfindingAgent(location).GetPathfinding();
        List<PathNode> path = pathfinding.GetPathToClosestReachableTile(transform.position, benchPosition);
        

        if (bench == null || path == null)
        {
            yield break;
        }

        foreach (PathNode node in path)
        {
            if (bench == null || path == null)
            {
                yield break;
            }

            Vector3 worldPos = pathfinding.GetGrid().GetWorldPosition(node.x, node.y);
            //transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            yield return MoveRoutine(worldPos);
        }

        // Reached the bench
        yield return Sit(bench);
        
    }

    private IEnumerator MoveRoutine(Vector3 endPoint)
    {
        Vector2 direction = (endPoint - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, endPoint);
        int failures = 0;
        while (distance > .15f && failures < 10)
        {
            if (Vector2.Distance(transform.position, endPoint) >= distance)
            {
                // Distance increased !!
                direction = (endPoint - transform.position).normalized;
                failures++;
            }

            distance = Vector2.Distance(transform.position, endPoint);

            animator.SetFloat("AnimMoveX", direction.normalized.x);
            animator.SetFloat("AnimMoveY", direction.normalized.y);
            animator.SetFloat("MovementMagnitude", direction.magnitude);
            rb.velocity = direction * speed;
            yield return new WaitForSeconds(.05f);
        }

        if (failures >= 10) {
            // Corrected position !!
            Debug.LogWarning("Corrected Position !! ");
            rb.position = endPoint;
        }
    }

    private IEnumerator Sit(Bench bench)
    {
        Stop();
        if (bench == null) { yield break; }

        if (bench.Interact(this)) { 
            while (true)
            {
                // A chance to do:
                // 50% Nothing
                // 50% Order something
                float n = Random.Range(0f, 1f);

                if (n < .5f)
                {
                    // Order something
                    yield return Order();
                }
                yield return new WaitForSeconds(1f);
            }
        }

        yield break;
    }

    private Ingredient desire;

    private IEnumerator Order()
    {
        desire = TavernStockController.GetRandomIngredient();

        if (desire == null )
        {
            // No stock
            print("NO STOCK ? ");
            yield break;
        }
        desireImage.sprite = desire.sprite;
        desireImage.gameObject.SetActive(true);
        
        bubbleAnimator.SetTrigger("Open");


        // PLACEHOLDER
        while (desire != null)
        {
            // WAIT X TICKS BEFORE CANCELLING THE ORDER
            yield return new WaitForSeconds(1f);
        }

        bubbleAnimator.SetTrigger("Close");

        // desire = null;
    }

    private void UpdateColors(Material m, ClothingItem i)
    {
        ClothingItem.ThreeColors c = i.GetRandomColor();

        m.SetColor("_Color1", c.primary);
        m.SetColor("_Color2", c.secondary);

        if (i.type.Equals(ClothingItem.ClothingType.Hair))
        {
            faceMaterial.SetColor("_Color3", c.primary);
        }

        if (i.type.Equals(ClothingItem.ClothingType.Faces))
        {
            body.color = c.primary;
            arms.color = c.primary;

            m.SetColor("_Color1", c.primary * ClothingController.GetTint());
        }

        // CONTINUE THIS: It is not as simple as setting c1 = primary and so on. For example, the primary hair color becomes the teritary facial color.
    }

    public void GenerateAOC()
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach (var kv in current)//instance.current)
        {
            overrides = kv.Value.GetAnimations(overrides);
        }

        aoc.ApplyOverrides(overrides);
        animator.runtimeAnimatorController = aoc;
    }

    public void Interact(GameObject itemHeld)
    {
        // If the npc wants something, try and take it.
        if (desire != null)
        {
            if (itemHeld != null && itemHeld.TryGetComponent(out StackableItem stackable))
            {
                if (stackable.GetIngredient().Equals(desire)) {
                    Satisfy(stackable);
                }
            }
        }
        // Otherwise, start a dialog
    }

    private void Satisfy(StackableItem item)
    {
        // Cancel desire
        desire = null;
        // Give MONEY
        PlayerInventory.ModifyGold(item.GetBasePrice());
        // Reduce stacks
        item.Consume();
        InventoryUI.instance.UpdateUI();
    }

}
