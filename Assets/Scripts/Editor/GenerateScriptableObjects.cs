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

                if (item.type.Equals(ClothingType.Hair))
                {
                    item.possibleColors = new PossibleColors();

                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("F5D18E"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("ECC567"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("E0AE3A"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("C57B4F"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("90481D"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("653112"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("E59776"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("CF724B"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("A8502A"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("C0AAAA"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("8C7171"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("353030"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("E261AB"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("6170E2"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("F3849B"));

                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("6B6F82"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("E26161"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("BA84F3"));


                    item.possibleColors.tertiary.Add(PossibleColors.ColorFromHexCode("FFFFFF"));

                    //item.possibleColors.secondary.Add()
                }

                if (item.type.Equals(ClothingType.Torso))
                {
                    item.possibleColors = new PossibleColors();

                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("61D5E2"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("61E28E"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("9EE261"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("E2E261"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("E2A261"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("E26161"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("E261AB"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("AB61E2"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("6170E2"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("E9E9E9"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("959595"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("484848"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("B98645"));

                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("61D5E2"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("61E28E"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("9EE261"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("E2E261"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("E2A261"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("E26161"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("E261AB"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("AB61E2"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("6170E2"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("E9E9E9"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("959595"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("484848"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("B98645"));

                    item.possibleColors.tertiary.Add(PossibleColors.ColorFromHexCode("FFFFFF"));
                }

                if (item.type.Equals(ClothingType.Legs))
                {
                    item.possibleColors = new PossibleColors();

                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("849CF3"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("84F3EE"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("90F384"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("F3EE84"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("F3B684"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("F3849B"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("F384EC"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("BA84F3"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("576AB0"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("E9E9E9"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("959595"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("484848"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("B98645"));

                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("849CF3"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("84F3EE"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("90F384"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("F3EE84"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("F3B684"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("F3849B"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("F384EC"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("BA84F3"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("576AB0"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("E9E9E9"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("959595"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("484848"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("B98645"));

                    item.possibleColors.tertiary.Add(PossibleColors.ColorFromHexCode("FFFFFF"));
                }

                if (item.type.Equals(ClothingType.Shoes))
                {
                    item.possibleColors = new PossibleColors();

                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("#986BA6"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("#6B75A6"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("#6BA69A"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("#A0A66B"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("#A66B80"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("#E9E9E9"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("#959595"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("#474747"));

                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("#986BA6"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("#6B75A6"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("#6BA69A"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("#A0A66B"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("#A66B80"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("#E9E9E9"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("#959595"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("#474747"));

                    item.possibleColors.tertiary.Add(PossibleColors.ColorFromHexCode("FFFFFF"));
                }

                if (item.type.Equals(ClothingType.Faces))
                {
                    item.possibleColors = new PossibleColors();

                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("##FFF7E8"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("##FFF0D2"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("##FFE0A5"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("##EEC474"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("##E2B256"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("##D69E34"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("##C88A14"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("##8C651A"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("###654F25"));
                    item.possibleColors.primary.Add(PossibleColors.ColorFromHexCode("###41341B"));

                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("##7499CF"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("##308C33"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("##90491D"));
                    item.possibleColors.secondary.Add(PossibleColors.ColorFromHexCode("##3F464F"));

                    item.possibleColors.tertiary.Add(PossibleColors.ColorFromHexCode("FFFFFF"));
                }



                //Debug.Log(item);
                AssetDatabase.CreateAsset(item, "Assets/Resources/Clothes/ScriptableObjects/" + item.name + ".asset");
            }
        }

    }
}
