using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GenerateScriptableObjects : MonoBehaviour
{

    public class AnimationWrapper
    {
        // Animations for left, right, front and back
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

        string str = "{\n";
        foreach(var kv in dictionary)
        {
            str += kv.Key + ": {\n";

            foreach (var kv2 in kv.Value)
            {
                str += kv2.Key + ": {\n";
                foreach (var kv3 in kv2.Value.container)
                {
                    str += kv3.Key + ": {";
                    str += "front: " + kv3.Value.front + ", back: " + kv3.Value.back + ", left: " + kv3.Value.left + ", right: " + kv3.Value.right + "}\n";
                }
                str += "}\n";
            }
            str += "}\n";
        }
        str += "}";
        Debug.Log(str);

    }
}
