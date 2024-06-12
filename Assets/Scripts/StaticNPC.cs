using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

public class StaticNPC : CharacterAbstract
{
    [SerializeField]
    protected SpriteRenderer body, arms, face, torso, hair, legs, shoes;
    protected Material torsoMaterial, hairMaterial, legsMaterial, faceMaterial, shoeMaterial;

    protected Dictionary<ClothingItem.ClothingType, ClothingItem> current;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(NPCController.GenerateRandomClothes());
    }

    public virtual void Initialize(Dictionary<ClothingItem.ClothingType, ClothingItem> clothes)
    {


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
