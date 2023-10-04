using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClothingController : MonoBehaviour
{

    private static ClothingController instance;
    private Dictionary<string, AnimationClip> defaults;

    private Dictionary<ClothingItem.ClothingType, ClothingItem> current;

    public ClothingItem[] testArray;

    private ClothingItem[] all;

    public AnimatorOverrideController aoc;
    public Animator animator;

    private Dictionary<ClothingItem.ClothingType, string> clothing_type_to_body_parts;

    public static readonly string[] BODY_PARTS = { "Legs", "Torso", "Faces", "Shoes", "Hair" };
    public static readonly string[] ORIENTATION = { "front", "back", "left", "right" };
    public static readonly string[] ANIMATIONS = { "idle", "sit", "hold", "walk" };

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
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

    private void Start()
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

    public static void GenerateAOC()
    {
        instance.aoc = new AnimatorOverrideController(instance.animator.runtimeAnimatorController);
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach (var kv in instance.testArray)//instance.current)
        {
            overrides = kv.GetAnimations(overrides);
        }

        instance.aoc.ApplyOverrides(overrides);
        instance.animator.runtimeAnimatorController = instance.aoc;
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
