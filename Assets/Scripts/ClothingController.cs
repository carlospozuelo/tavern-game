using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClothingController : MonoBehaviour
{

    private static ClothingController instance;
    private Dictionary<string, AnimationClip> defaults;

    private Dictionary<ClothingItem.ClothingType, ClothingItem> current;

    public List<ClothingItem> testArray;

    private ClothingItem[] all;

    private AnimatorOverrideController aoc;
    public Animator animator;

    public SpriteRenderer body, arms, face, torso, hair, legs, shoes;
    public Color greyTint;

    private Dictionary<ClothingItem.ClothingType, string> clothing_type_to_body_parts;

    public static readonly string[] BODY_PARTS = { "Legs", "Torso", "Faces", "Shoes", "Hair" };
    public static readonly string[] ORIENTATION = { "front", "back", "left", "right" };
    public static readonly string[] ANIMATIONS = { "idle", "sit", "hold", "walk" };


    private Color bodyColor, torsoColor, hairColor, legsColor;
    private Material faceMaterial, shoeMaterial;

    public static void UpdateColors()
    {
        instance.bodyColor = instance.body.color;
        instance.hairColor = instance.hair.color;
        instance.torsoColor = instance.torso.color;
        instance.legsColor = instance.legs.color;

        instance.faceMaterial = instance.face.material;
        instance.shoeMaterial = instance.shoes.material;
    }

    public static void UpdateColorsReverse()
    {
        instance.body.color = instance.bodyColor;
        instance.arms.color = instance.bodyColor;
        instance.torso.color = instance.torsoColor;
        instance.hair.color = instance.hairColor;
        instance.legs.color = instance.legsColor;

        instance.face.material = instance.faceMaterial;
        instance.shoes.material = instance.shoeMaterial;
    }

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            // Update sprite renderers and animator
            instance.animator = animator;
            instance.body = body;
            instance.arms = arms;
            instance.face = face;
            instance.torso = torso;
            instance.hair = hair;
            instance.legs = legs;
            instance.shoes = shoes;

            // Update colors
            UpdateColorsReverse();

            GenerateAOC();

            // Destroy new component
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
    }


    public static ClothingItem[] GetAll()
    {
        return instance.all;
    }

    public static List<ClothingItem> GetAll(ClothingItem.ClothingType type)
    {
        List<ClothingItem> list = new List<ClothingItem>();
        foreach (ClothingItem i in instance.all)
        {
            if (i.type.Equals(type))
            {
                list.Add(i);
            }
        }

        return list;
    }

    public static void SetAnimator(Animator a)
    {
        instance.animator = a;
    }

    public static string GetBodyPart(string name)
    {
        string body = "";
        foreach (string bodypart in BODY_PARTS)
        {
            if (name.Contains(bodypart)) { body = bodypart; break; }
        }
        return body;
    }

    public static string GetOrientation(string name)
    {
        string orientation = "";
        foreach (string o in ORIENTATION)
        {
            if (name.ToLower().Contains(o)) { orientation = o; break; }
        }
        return orientation;
    }

    public static string GetAnimation(string name)
    {
        string animation = "";
        foreach (string a in ANIMATIONS)
        {
            if (name.ToLower().Contains(a)) { animation = a; break; }
        }
        return animation;
    }

    private void Initialize()
    {
        AnimationClip[] list = Resources.LoadAll("Clothes/Animations/Default", typeof(AnimationClip)).Cast<AnimationClip>().ToArray();

        defaults = new Dictionary<string, AnimationClip>();

        foreach (AnimationClip clip in list)
        {
            string name = clip.name;

            if (!name.Equals("Empty"))
            {
                string body = GetBodyPart(name);
                string orientation = GetOrientation(name);
                string animation = GetAnimation(name);

                defaults[GetKey(body, orientation, animation)] = clip;
            } else
            {
                defaults[name] = clip;
            }
        }

        // TODO: Read data from file;
        instance.current = new Dictionary<ClothingItem.ClothingType, ClothingItem>();

        this.all = Resources.LoadAll("Clothes/ScriptableObjects", typeof(ClothingItem)).Cast<ClothingItem>().ToArray();

        clothing_type_to_body_parts = new Dictionary<ClothingItem.ClothingType, string>();

        clothing_type_to_body_parts[ClothingItem.ClothingType.Legs] = "Legs";
        clothing_type_to_body_parts[ClothingItem.ClothingType.Torso] = "Torso";
        clothing_type_to_body_parts[ClothingItem.ClothingType.Faces] = "Faces";
        clothing_type_to_body_parts[ClothingItem.ClothingType.Shoes] = "Shoes";
        clothing_type_to_body_parts[ClothingItem.ClothingType.Hair] = "Hair";

        GenerateAOC();
    }

    public static void SelectFirstColor(Color color, ClothingItem.ClothingType type)
    {
        if (type == ClothingItem.ClothingType.Torso)
        {
            // Update if multiple-colored clothings are developed.
            instance.torso.color = color;
        }
        if (type == ClothingItem.ClothingType.Legs)
        {
            instance.legs.color = color;
        }
        if (type == ClothingItem.ClothingType.Hair)
        {
            // Change hair color AND facial hair color
            instance.hair.color = color;
            instance.face.material.SetColor("_Color3", color);
        }
        if (type == ClothingItem.ClothingType.Shoes)
        {
            instance.shoes.material.SetColor("_Color1", color);
        }
        if (type == ClothingItem.ClothingType.Faces)
        {
            instance.arms.color = color;
            instance.body.color = color;
            // Facial details (nose, etc)
            instance.face.material.SetColor("_Color1", instance.greyTint * color);
        }
    }

    public static void SelectSecondColor(Color color, ClothingItem.ClothingType type)
    {
        if (type == ClothingItem.ClothingType.Shoes)
        {
            instance.shoes.material.SetColor("_Color2", color);
        }
        if (type == ClothingItem.ClothingType.Faces)
        {
            // Eyes 
            instance.face.material.SetColor("_Color2", color);
        }
    }

    public static void GenerateAOC()
    {
        instance.aoc = new AnimatorOverrideController(instance.animator.runtimeAnimatorController);
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        // TODO: Change test array and use current instead
        foreach (var kv in instance.testArray)//instance.current)
        {
            overrides = kv.GetAnimations(overrides);
        }

        instance.aoc.ApplyOverrides(overrides);
        instance.animator.runtimeAnimatorController = instance.aoc;
    }

    public static void Wear(ClothingItem item)
    {
        instance.current[item.type] = item;

        // TODO: Remove this (once AOC uses current instead of testArray)
        List<ClothingItem> newTestArray = new List<ClothingItem>();
        foreach (ClothingItem i in instance.testArray)
        {
            if (!i.type.Equals(item.type))
            {
                newTestArray.Add(i);
            }
        }

        newTestArray.Add(item);

        instance.testArray = newTestArray;
        GenerateAOC();
    }

    public static AnimationClip GetDefaultClip(ClothingItem.ClothingType type, string orientation, string animation)
    {
        string key = GetKey(instance.clothing_type_to_body_parts[type], orientation, animation);
        if (instance.defaults.ContainsKey(key)) {
            return instance.defaults[key];
        } else
        {
            return instance.defaults["Empty"];
        }
    }


    private static string GetKey(string body, string orientation, string animation)
    {
        return body + " " + orientation + " " + animation;
    }


}
