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
        Debug.Log("Generating animation for " + name + "... ");
        if (name.Contains("Torso"))
        {
            AnimationClip idle = new AnimationClip();

            EditorCurveBinding spriteBinding = new EditorCurveBinding();
            spriteBinding.type = typeof(SpriteRenderer);
            spriteBinding.path = "Torso";
            spriteBinding.propertyName = "m_Sprite";
            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[2];
            spriteKeyFrames[0] = new ObjectReferenceKeyframe();
            spriteKeyFrames[0].time = 0;
            spriteKeyFrames[0].value = meta[0];

            idle.name = name + "_" + index;
            AnimationUtility.SetObjectReferenceCurve(idle, spriteBinding, spriteKeyFrames);
            AssetDatabase.CreateAsset(idle, "Assets/Debug/" + idle.name + ".anim");
        }
    }
    

}

