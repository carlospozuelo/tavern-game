using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static ClothingItem;

public class GenerateScriptableObjects : MonoBehaviour
{

    public class AnimationWrapper
    {
        // Animations for (idle, sit, hold, walk) each -> left, right, front and back
        public Dictionary<string, ClothingItem.AnimationContainer> container;

        public AnimationWrapper()
        {
            container = new Dictionary<string, ClothingItem.AnimationContainer>();
        }
    }

    [MenuItem("Tools/Generate ScriptableObjects")]
    static void Generate()
    {
        AnimationClip[] list = Resources.LoadAll("Clothes/Animations", typeof(AnimationClip)).Cast<AnimationClip>().ToArray();

        // { Torso: [ 0: { idle: { front: clip, right: clip, left: clip, back: clip }, 1: {}, ... ], Hair: { ... } ], ... }

        Dictionary<string, Dictionary<int, AnimationWrapper>> dictionary = new Dictionary<string, Dictionary<int, AnimationWrapper>>();

        foreach (AnimationClip clip in list)
        {
            string name = clip.name;
            string[] split = name.Split(" ");
            if (!name.Equals("Empty") && split.Length == 4)
            {
                string body = ClothingController.GetBodyPart(name);
                string orientation = ClothingController.GetOrientation(name);
                string animation = ClothingController.GetAnimation(name);
                
                int number = int.Parse(split[1]);

                if (!dictionary.ContainsKey(body))
                {
                    dictionary[body] = new Dictionary<int, AnimationWrapper>();
                }

                if (!dictionary[body].ContainsKey(number))
                {
                    dictionary[body][number] = new AnimationWrapper();
                }

                if (!dictionary[body][number].container.ContainsKey(animation))
                {
                    dictionary[body][number].container[animation] = new ClothingItem.AnimationContainer();
                }

                dictionary[body][number].container[animation].StoreAnimation(clip, orientation);
            }
        }

        foreach (var kv in dictionary)
        {
            foreach (var kv2 in kv.Value)
            {
                ClothingItem item = ClothingItem.CreateInstance(kv.Key, kv.Key + " " + kv2.Key.ToString("00"));

                foreach (var kv3 in kv2.Value.container)
                {
                    item.SetAnimationContainer(kv3.Value, kv3.Key.ToLower());

                }
                //Debug.Log(item);
                AssetDatabase.CreateAsset(item, "Assets/Resources/Clothes/ScriptableObjects/" + item.name + ".asset");
            }
        }

    }
}
