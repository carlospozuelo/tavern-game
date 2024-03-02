using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : CharacterAbstract
{
    [SerializeField]
    private SpriteRenderer body, arms, face, torso, hair, legs, shoes;
    private Material torsoMaterial, hairMaterial, legsMaterial, faceMaterial, shoeMaterial;

    private Dictionary<ClothingItem.ClothingType, ClothingItem> current;
    private string location;

    public void SetLocation(string location) { this.location = location; }
    public string GetLocation() { return location; }

    

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
            Bench bench = NPCController.GetRandomBench();
            if (bench != null) {
                loggedInactivity = false;
                yield return WalkTowardsBench(bench);
            }
            else
            {
                // Nothing else to do
                if (!loggedInactivity)
                {
                    Debug.LogWarning("There's nothing for the npc to do! Wander around?");
                    loggedInactivity = true;
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // TODO: Walk towards a NON selected bench
    private IEnumerator WalkTowardsBench(Bench bench)
    {
        Vector3 benchPosition = bench.GetPosition();
        Pathfinding pathfinding = LocationController.GetPathfindingAgent(location).GetPathfinding();
        List<PathNode> path = pathfinding.GetPathToClosestReachableTile(transform.position, benchPosition);

        while (true)
        {
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
                // TODO: Modify movement
                Vector3 worldPos = pathfinding.GetGrid().GetWorldPosition(node.x, node.y);
                transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
                yield return new WaitForSeconds(.5f);
            }

            // Reached the bench
            yield return Sit(bench);
        }
    }

    private IEnumerator Sit(Bench bench)
    {
        if (bench == null) { yield break; }
        Debug.Log("Sitting for eternity");
        bench.Interact(this);
        while (true)
        {
            yield return new WaitForSeconds(1f);
        }
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

}
