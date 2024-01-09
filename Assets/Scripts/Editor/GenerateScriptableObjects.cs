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
        Sprite[] sprites = Resources.LoadAll("", typeof(Sprite)).Cast<Sprite>().ToArray();

        // Maps BODY PART and NUMBER to the default sprite (x00)
        Dictionary<(string,string), Sprite> defaultSprites = new Dictionary<(string, string), Sprite>();

        foreach (Sprite s in sprites)
        {
            if (s.name.Contains("Front") && s.name.Contains("PREVIEW"))
            {
                string bodyPart = s.name.Split(" ")[0];
                string number = s.name.Split("_")[1].Split(" ")[0];
                //string number = tmp.Substring(0, tmp.Length - 3);

                defaultSprites.Add((bodyPart, number), s);
                Debug.Log("k: " + (bodyPart, number) + ", v: " + defaultSprites[(bodyPart, number)]);
            }
        }
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

                (string, string) key = (item.type.ToString(), item.Name.Split(" ")[1]);
                if (defaultSprites.TryGetValue(key, out Sprite s))
                {
                    item.sprite = s;
                }

                //Debug.Log(item);
                AssetDatabase.CreateAsset(item, "Assets/Resources/Clothes/ScriptableObjects/" + item.name + ".asset");
            }
        }

    }
}
