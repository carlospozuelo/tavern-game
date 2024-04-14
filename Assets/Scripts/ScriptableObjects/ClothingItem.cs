using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClothingItem", menuName = "ScriptableObjects/ClothingItems", order = 1)]
public class ClothingItem : ScriptableObject
{
    [Serializable]
    public class AnimationContainer
    {
        public AnimationClip front, back, left, right;

        public List<KeyValuePair<AnimationClip, AnimationClip>> GetAnimations(List<KeyValuePair<AnimationClip, AnimationClip>> overrides, ClothingType type, string anim)
        {
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(ClothingController.GetDefaultClip(type, "front", anim), front));
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(ClothingController.GetDefaultClip(type, "back", anim), back));
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(ClothingController.GetDefaultClip(type, "left", anim), left));
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(ClothingController.GetDefaultClip(type, "right", anim), right));

            return overrides;
        }

        public void StoreAnimation(AnimationClip clip, string direction)
        {
            if (direction.Equals("front")) { front = clip; return; }
            if (direction.Equals("left")) { left = clip; return; }
            if (direction.Equals("right")) { right = clip; return; }
            if (direction.Equals("back")) { back = clip; return; }
        }

        public override string ToString()
        {
            string front_name = front == null ? "null" : front.name;
            string right_name = right == null ? "null" : right.name;
            string left_name = left == null ? "null" : left.name;
            string back_name = back == null ? "null" : back.name;

            string str = "front: " + front_name + ",\n" +
                            "back: " + back_name + ",\n" +
                            "left: " + left_name + ",\n" +
                            "right: " + right_name + "\n";

            return str;
        }
    }

    // "Constructor"
    public static ClothingItem CreateInstance(string type, string name)
    {
        var data = CreateInstance<ClothingItem>();

        data.type = ToClothingType(type);
        data.name = name;
        data.Name = name;
        //data.sprite = sprite;

        data.idle = new AnimationContainer();
        data.hold = new AnimationContainer();
        data.walk = new AnimationContainer();
        data.sit = new AnimationContainer();

        return data;
    }

    public enum ClothingType
    {
        Torso, Legs, Hair, Faces, Shoes
    }

    public static ClothingType ToClothingType(string name)
    {
        name = name.ToLower();

        if (name.Equals("torso")) return ClothingType.Torso;
        if (name.Equals("legs")) return ClothingType.Legs;
        if (name.Equals("hair")) return ClothingType.Hair;
        if (name.Equals("shoes")) return ClothingType.Shoes;
        if (name.Equals("faces")) return ClothingType.Faces;

        return ClothingType.Torso;
    }

    public ClothingType type;

    public AnimationContainer idle, hold, walk, sit, bonk;


    [System.Serializable]
    public class ThreeColors
    {
        public Color primary, secondary, tertiary;

        public ThreeColors() { }

        public ThreeColors(Color primary, Color secondary, Color tertiary)
        {
            this.primary = primary;
            this.secondary = secondary;
            this.tertiary = tertiary;
        }

    }

    [System.Serializable]
    public class PossibleColors
    {
        public List<Color> primary, secondary, tertiary;

        public PossibleColors() { 
            primary = new List<Color>();
            secondary = new List<Color>();
            tertiary = new List<Color>();
        }

        public static Color ColorFromHexCode(string code)
        {
            // Remove the '#' character from the hex code
            code = code.TrimStart('#');

            // Parse the R, G, B, A values from the hex code
            float r = Convert.ToInt32(code.Substring(0, 2), 16) / 255.0f;
            float g = Convert.ToInt32(code.Substring(2, 2), 16) / 255.0f;
            float b = Convert.ToInt32(code.Substring(4, 2), 16) / 255.0f;

            float a = 1.0f; // Default alpha value

            // Check if the hex code includes transparency information
            if (code.Length == 8)
            {
                int alpha = Convert.ToInt32(code.Substring(6, 2), 16);
                a = alpha / 255.0f;
            }

            // Create and return the Color object
            return new Color(r, g, b, a);
        }
    }

    public PossibleColors possibleColors;

    public ThreeColors GetRandomColor()
    {
        return new ThreeColors(possibleColors.primary[UnityEngine.Random.Range(0, possibleColors.primary.Count)],
                               possibleColors.secondary[UnityEngine.Random.Range(0, possibleColors.secondary.Count)],
                               possibleColors.tertiary[UnityEngine.Random.Range(0, possibleColors.tertiary.Count)]);
    }

    public Sprite sprite;

    public string Name;



    public AnimationContainer GetAnimationContainer(string name)
    {
        if (name.Equals("idle")) return idle;
        if (name.Equals("hold")) return hold;
        if (name.Equals("walk") || name.Equals("run")) return walk;
        if (name.Equals("sit")) return sit;

        return null;
    }

    public void SetAnimationContainer(AnimationContainer c, string name)
    {
        if (name.Equals("idle")) idle = c;
        if (name.Equals("hold")) hold = c;
        if (name.Equals("walk") || name.Equals("run")) walk = c;
        if (name.Equals("sit")) sit = c;

        if (name.Equals("bonk")) bonk = c;
    }

    public List<KeyValuePair<AnimationClip, AnimationClip>> GetAnimations(List<KeyValuePair<AnimationClip, AnimationClip>> overrides)
    {
        overrides = idle.GetAnimations(overrides, type, "idle");
        overrides = hold.GetAnimations(overrides, type, "hold");
        overrides = walk.GetAnimations(overrides, type, "walk");
        overrides = sit.GetAnimations(overrides, type, "sit");
        overrides = bonk.GetAnimations(overrides, type, "bonk");


        return overrides;
    }

    public override string ToString()
    {
        string str = type + ", " + Name + ": {\n" +
            "idle: {\n" + idle + "},\n" +
            "hold: {\n" + hold + "},\n" +
            "walk: {\n" + walk + "},\n" +
            "sit: {\n" + sit + "}";

        return str;
    }
}
