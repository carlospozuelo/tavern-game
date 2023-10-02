using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;

public class GenerateAnimations : MonoBehaviour
{
    [MenuItem("Tools/Read Animations")]
    static void ReadAnimDebug()
    {
        AnimationClip[] animations = Resources.LoadAll("", typeof(AnimationClip)).Cast<AnimationClip>().ToArray();

        foreach (AnimationClip clip in animations)
        {
            Debug.Log("Analyzing: " + clip.name);
            foreach (EditorCurveBinding e in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                Debug.Log(e.propertyName);
                Debug.Log(e.path);
            }
        }
    }

    static void ReadSprites()
    {
        Sprite[] sprites = Resources.LoadAll("", typeof(Sprite)).Cast<Sprite>().ToArray();
        Dictionary<string, Dictionary<int, List<Sprite>>> dic = new Dictionary<string, Dictionary<int, List<Sprite>>>();

        foreach (var sprite in sprites)
        {
            var result = ExtractData(sprite.name);
            string key = result.Text1 + " " + result.Text2;

            if (!dic.ContainsKey(key))
            {
                dic.Add(key, new Dictionary<int, List<Sprite>>());
            }

            if (!dic[key].ContainsKey(result.Number1))
            {
                dic[key].Add(result.Number1, new List<Sprite>());
            }

            dic[key][result.Number1].Add(sprite);
        }

        foreach (var keyvalue in dic)
        {
            foreach (var kv in keyvalue.Value)
            {
                GenerateAnimation(kv.Value, keyvalue.Key, kv.Key);
            }
        }

        
    }

    static (string Text1, string Text2, int Number1, int Number2) ExtractData(string input)
    {
        // Define the regular expression pattern
        string pattern = @"^(.*?)\s(.*?)_(\d+)x(\d+)$";

        // Use Regex.Match to find the first match in the input string
        Match match = Regex.Match(input, pattern);

        if (match.Success)
        {
            // Extract the captured groups
            string text1 = match.Groups[1].Value;
            string text2 = match.Groups[2].Value;
            int number1 = int.Parse(match.Groups[3].Value);
            int number2 = int.Parse(match.Groups[4].Value);

            return (text1, text2, number1, number2);
        }
        else
        {
            // Return default values or handle the case when the input doesn't match the pattern
            return (string.Empty, string.Empty, 0, 0);
        }
    }

    [MenuItem("Tools/Generate Animations")]
    static void GenerateAnimation()
    {
        Debug.Log("Generating Animations");

        int width = 32;
        int height = 64;

        string folder = "Clothes";

        Texture2D[] textures = Resources.LoadAll(folder, typeof(Texture2D)).Cast<Texture2D>().ToArray();

        foreach (Texture2D texture in textures)
        {
            Debug.Log("[Computing] " + texture);

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Multiple;

            ti.spritePixelsPerUnit = 32;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;

            List<SpriteMetaData> meta = new List<SpriteMetaData>();
            ti.spritesheet = new SpriteMetaData[0];

            for (int i = texture.height; i > 0; i -= height)
            {
                int rowNum = (texture.height - i) / height;
                
                for (int j = 0; j < texture.width; j += width) {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = ((int)SpriteAlignment.Center);

                    int colNum = j / width;

                    smd.name = texture.name + "_" + rowNum + "x" + colNum;
                    smd.rect = new Rect(j, i - height, width, height);

                    meta.Add(smd);
                }
            }

            ti.spritesheet = meta.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            Debug.Log("[Computing] " + texture + " ok");
        }

        ReadSprites();
    }

    
    private static void GenerateAnimation(List<Sprite> meta, string name, int index)
    {

        const string IDLE = "idle";
        // const string RUN = "run";
        const string HOLD = "hold";
        const string SIT = "sit";

        const string TORSO = "Torso";
        const string LEGS = "Legs";
        const string HAIR = "Hair";
        const string FACES = "Faces";

        Debug.Log("Generating animation for " + name + "... ");
        if (name.Contains(TORSO))
        {
            GenerateIdleAnimation(meta[0], name, index, TORSO, IDLE);
            GenerateRunAnimation(meta, name, index, TORSO);
            GenerateIdleAnimation(meta[4], name, index, TORSO, SIT);
            GenerateIdleAnimation(meta[3], name, index, TORSO, HOLD);
        }

        if (name.Contains(LEGS))
        {
            GenerateIdleAnimation(meta[0], name, index, LEGS, IDLE);
            GenerateRunAnimation(meta, name, index, LEGS);
            GenerateIdleAnimation(meta[3], name, index, LEGS, SIT);
        }

        if (name.Contains(HAIR))
        {
            GenerateIdleAnimation(meta[0], name, index, HAIR, IDLE);
            GenerateIdleAnimation(meta[1], name, index, HAIR, SIT);
        }

        if(name.Contains(FACES))
        {
            GenerateBlinkAnimation(meta[0], meta[1], name, index, FACES, IDLE);
            GenerateBlinkAnimation(meta[2], meta[3], name, index, FACES, SIT);
        }
    }


    private static void GenerateGenericAnimation(string name, int index, string path, string anim, ObjectReferenceKeyframe[] spriteKeyFrames)
    {
        AnimationClip clip = new AnimationClip();
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = path;
        spriteBinding.propertyName = "m_Sprite";

        var split = name.Split(" ");

        clip.name = split[0] + " " + index + " " + split[1] + " " + anim;
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;

        AnimationUtility.SetAnimationClipSettings(clip, settings);
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
        AssetDatabase.CreateAsset(clip, "Assets/Debug/" + clip.name + ".anim");
    }

    private static void GenerateBlinkAnimation(Sprite sprite, Sprite blink, string name, int index, string path, string anim)
    {
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[3];
        spriteKeyFrames[0] = new ObjectReferenceKeyframe();
        spriteKeyFrames[0].time = 0;
        spriteKeyFrames[0].value = sprite;

        spriteKeyFrames[1] = new ObjectReferenceKeyframe();
        spriteKeyFrames[1].time = 4;
        spriteKeyFrames[1].value = blink;

        spriteKeyFrames[2] = new ObjectReferenceKeyframe();
        spriteKeyFrames[2].time = 4f + (1f/30f);
        spriteKeyFrames[2].value = sprite;

        GenerateGenericAnimation(name, index, path, anim, spriteKeyFrames);
    }

    private static void GenerateRunAnimation(List<Sprite> meta, string name, int index, string path)
    {
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[5];
        spriteKeyFrames[0] = new ObjectReferenceKeyframe();
        spriteKeyFrames[0].time = 0;
        spriteKeyFrames[0].value = meta[1];

        spriteKeyFrames[1] = new ObjectReferenceKeyframe();
        spriteKeyFrames[1].time = .25f;
        spriteKeyFrames[1].value = meta[2];

        spriteKeyFrames[2] = new ObjectReferenceKeyframe();
        spriteKeyFrames[2].time = .5f;
        spriteKeyFrames[2].value = meta[1];

        spriteKeyFrames[3] = new ObjectReferenceKeyframe();
        spriteKeyFrames[3].time = .75f;
        spriteKeyFrames[3].value = meta[2];

        spriteKeyFrames[4] = new ObjectReferenceKeyframe();
        spriteKeyFrames[4].time = 1;
        spriteKeyFrames[4].value = meta[1];

        GenerateGenericAnimation(name, index, path, "run", spriteKeyFrames);
    }

    private static void GenerateIdleAnimation(Sprite sprite, string name, int index, string path, string anim)
    {
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[2];
        spriteKeyFrames[0] = new ObjectReferenceKeyframe();
        spriteKeyFrames[0].time = 0;
        spriteKeyFrames[0].value = sprite;

        spriteKeyFrames[1] = new ObjectReferenceKeyframe();
        spriteKeyFrames[1].time = 1;
        spriteKeyFrames[1].value = sprite;

        GenerateGenericAnimation(name, index, path, anim, spriteKeyFrames);
    }

}

