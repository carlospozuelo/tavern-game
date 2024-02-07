using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer body, arms, face, torso, hair, legs, shoes;
    private Material torsoMaterial, hairMaterial, legsMaterial, faceMaterial, shoeMaterial;
    private Color bodyColor;

    private Dictionary<ClothingItem.ClothingType, ClothingItem> current;
    private string location;

    public void SetLocation(string location) { this.location = location; }
    public string GetLocation() { return location; }

    [SerializeField]
    private Animator animator;

    public void Initialize(Dictionary<ClothingItem.ClothingType, ClothingItem> clothes) {
        bodyColor = body.color;

        torsoMaterial = torso.material;
        hairMaterial = hair.material;
        legsMaterial = legs.material;
        faceMaterial = face.material;
        shoeMaterial = shoes.material;

        current = clothes;
        GenerateAOC();
    }

    private void UpdateColors(Material m, ClothingItem i)
    {
        ClothingItem.ThreeColors c = i.GetRandomColor();

        m.SetColor("_Color1", c.primary);
        m.SetColor("_Color2", c.secondary);
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
