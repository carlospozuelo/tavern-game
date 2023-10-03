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
            Debug.Log(clip + " " + direction);
            if (direction.Equals("front")) { front = clip; return; }
            if (direction.Equals("left")) { left = clip; return; }
            if (direction.Equals("right")) { right = clip; return; }
            if (direction.Equals("back")) { back = clip; return; }
        }
    }

    public ClothingItem()
    {
        idle = new AnimationContainer();
        hold = new AnimationContainer();
        walk = new AnimationContainer();
        sit = new AnimationContainer();
    }

    public enum ClothingType
    {
        Torso, Legs, Hair, Faces, Shoes
    }

    public ClothingType type;

    public AnimationContainer idle, hold, walk, sit;

    public Color primary, secondary, tertiary;

    public List<KeyValuePair<AnimationClip, AnimationClip>> GetAnimations(List<KeyValuePair<AnimationClip, AnimationClip>> overrides)
    {
        overrides = idle.GetAnimations(overrides, type, "idle");
        overrides = hold.GetAnimations(overrides, type, "hold");
        overrides = walk.GetAnimations(overrides, type, "walk");
        overrides = sit.GetAnimations(overrides, type, "sit");


        return overrides;
    }
}
