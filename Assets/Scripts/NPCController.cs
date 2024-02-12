using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private static NPCController instance;

    [SerializeField]
    private HashSet<Interactuable> interactuablesForNPCS;

    [SerializeField]
    private GameObject npcPrefab;
    private bool initialized = false;
    private void Start()
    {
        if (initialized) return;

        interactuablesForNPCS = new HashSet<Interactuable>();
        initialized = true;
    }

    public static void AddInteractuableForNPC(Interactuable i)
    {
        if (!instance.initialized)
        {
            instance.Start();
        }
        if (!instance.interactuablesForNPCS.Contains(i))
        {
            instance.interactuablesForNPCS.Add(i);
        }
    }

    public static void RemoveInteractuableForNPC(Interactuable i)
    {
        if (instance.interactuablesForNPCS.Contains(i))
        {
            instance.interactuablesForNPCS.Remove(i);
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }

    public static void SpawnNPC()
    {
        // The first approach just spawns npcs in the tavern. This is provisional.
        GameObject go = Instantiate(instance.npcPrefab,new Vector3(5f,-1.72f, 2.5f) + new Vector3(Random.Range(0f,1f), Random.Range(0f,1f)), Quaternion.identity, LocationController.GetLocation("Tavern").transform);

        NPC npc = go.GetComponent<NPC>();
        npc.SetLocation("Tavern");

        npc.Initialize(GenerateRandomClothes());
    }

    public static Dictionary<ClothingItem.ClothingType, ClothingItem> GenerateRandomClothes()
    {
        Dictionary<ClothingItem.ClothingType, ClothingItem> clothes = new Dictionary<ClothingItem.ClothingType, ClothingItem> ();

        clothes[ClothingItem.ClothingType.Hair] = ClothingController.GetRandomClothingItem(ClothingItem.ClothingType.Hair);
        clothes[ClothingItem.ClothingType.Legs] = ClothingController.GetRandomClothingItem(ClothingItem.ClothingType.Legs);
        clothes[ClothingItem.ClothingType.Torso] = ClothingController.GetRandomClothingItem(ClothingItem.ClothingType.Torso);
        clothes[ClothingItem.ClothingType.Shoes] = ClothingController.GetRandomClothingItem(ClothingItem.ClothingType.Shoes);
        clothes[ClothingItem.ClothingType.Faces] = ClothingController.GetRandomClothingItem(ClothingItem.ClothingType.Faces);

        return clothes;

    }



}
