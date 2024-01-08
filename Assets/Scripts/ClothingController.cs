using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class PlayerClothingData
{
    [JsonProperty]
    public string playerName, playerPronouns;
    [JsonProperty]
    public Dictionary<ClothingItem.ClothingType, ClothingData> data;
    [JsonProperty]
    private SerializableColor skinTone;

    public Color GetSkintone() { return skinTone.GetColor(); }
    public void SetColor(Color c)
    {
        skinTone = new SerializableColor(c);
    }

    public override string ToString()
    {
        string s = "{\ndata:\n{";
        if (data != null)
        {
            foreach (var keypair in data)
            {
                s += keypair.Key + ": " + keypair.Value + ",\n";
            }
        }
        s += ",\nskinTone: " + GetSkintone();

        return s;
    }

    
}


public class ClothingController : MonoBehaviour
{

    private static ClothingController instance;
    private Dictionary<string, AnimationClip> defaults;

    private Dictionary<ClothingItem.ClothingType, ClothingItem> current;

    private ClothingItem[] all;

    private AnimatorOverrideController aoc;
    public Animator animator;

    public SpriteRenderer body, arms, face, torso, hair, legs, shoes;
    public Color greyTint;

    public string playerName, pronouns;

    private Dictionary<ClothingItem.ClothingType, string> clothing_type_to_body_parts;

    [SerializeField]
    private GameObject clothingPrefab;

    private Dictionary<string, ClothingItem> allClothingItems;

    public static ClothingItem GetClothingItem(string s)
    {
        return instance.allClothingItems[s];
    }

    public static GameObject GenerateClothingObject(ClothingItem item, ClothingItem.ThreeColors colors)
    {
        GameObject gameObject = Instantiate(instance.clothingPrefab, Vector3.zero, Quaternion.identity,instance.transform);

        Clothing clothing = gameObject.GetComponent<Clothing>();
        clothing.Initialize(item, colors);

        gameObject.name = clothing.GetName();

        return gameObject;
    }

    public static Dictionary<ClothingItem.ClothingType, GameObject> GenerateClothingObjects()
    {
        Dictionary<ClothingItem.ClothingType, GameObject> res = new Dictionary<ClothingItem.ClothingType, GameObject>();

        res[ClothingItem.ClothingType.Legs] = GenerateClothingObject(instance.current[ClothingItem.ClothingType.Legs], instance.GetThreeColors(instance.legsMaterial));
        res[ClothingItem.ClothingType.Shoes] = GenerateClothingObject(instance.current[ClothingItem.ClothingType.Shoes], instance.GetThreeColors(instance.shoeMaterial));
        res[ClothingItem.ClothingType.Torso] = GenerateClothingObject(instance.current[ClothingItem.ClothingType.Torso], instance.GetThreeColors(instance.torsoMaterial));

        return res;
    }

    public static readonly string[] BODY_PARTS = { "Legs", "Torso", "Faces", "Shoes", "Hair" };
    public static readonly string[] ORIENTATION = { "front", "back", "left", "right" };
    public static readonly string[] ANIMATIONS = { "idle", "sit", "hold", "walk" };


    private Color bodyColor;
    private Material torsoMaterial, hairMaterial, legsMaterial, faceMaterial, shoeMaterial;

    public ClothingItem.ThreeColors GetThreeColors(Material m)
    {
        ClothingItem.ThreeColors colors = new ClothingItem.ThreeColors();

        colors.primary = m.GetColor("_Color1");
        colors.secondary = m.GetColor("_Color2");
        colors.tertiary = m.GetColor("_Color3");

        return colors;
    }

    public Material GetMaterial(ClothingItem.ClothingType t)
    {
        if (t == ClothingItem.ClothingType.Torso) { return torsoMaterial; }
        if (t == ClothingItem.ClothingType.Hair) { return hairMaterial; }
        if (t == ClothingItem.ClothingType.Legs) { return legsMaterial; }
        if (t == ClothingItem.ClothingType.Faces) { return faceMaterial; }
        if (t == ClothingItem.ClothingType.Shoes) { return shoeMaterial; }

        return null;
    }

    private void Start()
    {
        Master data = MasterData.Read();

        if (data != null)
        {
            Deserialize(data.clothingData);
        }
    }

    public static PlayerClothingData Serialize()
    {
        PlayerClothingData playerData = new PlayerClothingData();

        // Serialize
        playerData.data = new Dictionary<ClothingItem.ClothingType, ClothingData>();
        playerData.SetColor(instance.bodyColor);

        foreach (var keypair in instance.current)
        {
            Material m;
            if (keypair.Key == ClothingItem.ClothingType.Torso) { m = instance.torsoMaterial; }
            else if (keypair.Key == ClothingItem.ClothingType.Legs) { m = instance.legsMaterial; }
            else if (keypair.Key == ClothingItem.ClothingType.Hair) { m = instance.hairMaterial; }
            else if (keypair.Key == ClothingItem.ClothingType.Faces) { m = instance.faceMaterial; }
            else m = instance.shoeMaterial;

            playerData.data[keypair.Key] = new ClothingData(keypair.Value.Name, m.GetColor("_Color1"), m.GetColor("_Color2"), m.GetColor("_Color3"));
        }

        playerData.playerPronouns = instance.pronouns;
        playerData.playerName = instance.playerName;

        return playerData;
    }

    public void Deserialize(PlayerClothingData data)
    {
        instance.bodyColor = data.GetSkintone();

        foreach (var keypair in data.data)
        {
            Material m;
            if (keypair.Key == ClothingItem.ClothingType.Torso) { m = instance.torsoMaterial; }
            else if (keypair.Key == ClothingItem.ClothingType.Legs) { m = instance.legsMaterial; }
            else if (keypair.Key == ClothingItem.ClothingType.Hair) { m = instance.hairMaterial; }
            else if (keypair.Key == ClothingItem.ClothingType.Faces) { m = instance.faceMaterial; }
            else m = instance.shoeMaterial;
            // Restore color
            m.SetColor("_Color1", keypair.Value.GetColor1());
            m.SetColor("_Color2", keypair.Value.GetColor2());
            m.SetColor("_Color3", keypair.Value.GetColor3());

            // Store item for future AOC generation
            ClothingItem item = keypair.Value.GetClothingItem();
            instance.current[keypair.Key] = item;
        }

        GenerateAOC();
    }

    public static void UpdateNameAndPronouns(string playername, string pronouns)
    {
        instance.playerName = playername;
        instance.pronouns = pronouns;
    }

    public static void UpdateColors()
    {
        instance.bodyColor = instance.body.color;
        instance.hairMaterial = instance.hair.material;
        instance.torsoMaterial = instance.torso.material;
        instance.legsMaterial = instance.legs.material;

        instance.faceMaterial = instance.face.material;
        instance.shoeMaterial = instance.shoes.material;
    }

    public static void UpdateColorsReverse()
    {
        if (instance.torsoMaterial != null)
        {
            instance.body.color = instance.bodyColor;
            instance.arms.color = instance.bodyColor;
            instance.torso.material = instance.torsoMaterial;
            instance.hair.material = instance.hairMaterial;
            instance.legs.material = instance.legsMaterial;

            instance.face.material = instance.faceMaterial;
            instance.shoes.material = instance.shoeMaterial;
        } else
        {
            UpdateColors();
        }
        
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

        allClothingItems = new Dictionary<string, ClothingItem>();
        foreach (ClothingItem c in this.all)
        {
            allClothingItems.Add(c.Name, c);
        }

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
            instance.torso.material.SetColor("_Color1", color);
        }
        if (type == ClothingItem.ClothingType.Legs)
        {
            instance.legs.material.SetColor("_Color1", color);
        }
        if (type == ClothingItem.ClothingType.Hair)
        {
            // Change hair color AND facial hair color
            instance.hair.material.SetColor("_Color1", color);
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
        if (type == ClothingItem.ClothingType.Torso)
        {
            instance.torso.material.SetColor("_Color2", color);
        }
        if (type == ClothingItem.ClothingType.Legs)
        {
            instance.legs.material.SetColor("_Color2", color);
        }
        if (type == ClothingItem.ClothingType.Hair)
        {
            // Change hair color AND facial hair color
            instance.hair.material.SetColor("_Color2", color);
        }
    }

    public static void GenerateAOC()
    {
        instance.aoc = new AnimatorOverrideController(instance.animator.runtimeAnimatorController);
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        // TODO: Change test array and use current instead
        foreach (var kv in instance.current)//instance.current)
        {
            overrides = kv.Value.GetAnimations(overrides);
        }

        instance.aoc.ApplyOverrides(overrides);
        instance.animator.runtimeAnimatorController = instance.aoc;
    }

    public static void Wear(ClothingItem item)
    {
        instance.current[item.type] = item;

        GenerateAOC();
    }

    public static void Wear(ClothingItem item, ClothingItem.ThreeColors colors)
    {

        Material m = instance.GetMaterial(item.type);

        if (m != null)
        {
            m.SetColor("_Color1", colors.primary);
            m.SetColor("_Color2", colors.secondary);
            m.SetColor("_Color3", colors.tertiary);

        }

        Wear(item);

        
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
