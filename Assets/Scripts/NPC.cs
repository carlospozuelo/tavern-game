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
