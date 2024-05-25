using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private static NPCController instance;

    private HashSet<Interactuable> interactuablesForNPCS;

    private HashSet<Bench> benchesForNPCS;

    [SerializeField]
    private GameObject npcPrefab;
    private bool initialized = false;
    private void Start()
    {
        if (initialized) return;

        interactuablesForNPCS = new HashSet<Interactuable>();
        benchesForNPCS = new HashSet<Bench>();
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

    public static void AddBenchForNPC(Bench b)
    {
        if (b.IsBusy()) { return; }

        if (!instance.initialized)
        {
            instance.Start();
        }
        if (!instance.benchesForNPCS.Contains(b))
        {
            instance.benchesForNPCS.Add(b);
        }

        // Debug();

    }

    public static Bench GetRandomBench()
    {
        if (instance.benchesForNPCS.Count == 0) { return null; }

        return instance.benchesForNPCS.ElementAt(Random.Range(0, instance.benchesForNPCS.Count));
    }

    public static Bench PopRandomBench()
    {
        Bench bench = GetRandomBench();

        instance.benchesForNPCS.Remove(bench);

        return bench;
    }

    private static void Debug()
    {
        string str = "";
        foreach (var bench in instance.benchesForNPCS)
        {
            str += bench + " ";
        }

        DebugPanelUI.instance.Debug(str);
    }

    public static void RemoveBenchForNPC(Bench b)
    {
        if (instance.benchesForNPCS.Contains(b))
        {
            instance.benchesForNPCS.Remove(b);
        }

        // Debug();
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
        
        GameObject go = Instantiate(instance.npcPrefab, Portal.GetPortal("Tavern entrance").GetPosition() + new Vector3(Random.Range(0f,1f), Random.Range(0f,1f), 2.5f), Quaternion.identity, LocationController.GetLocation("Tavern").transform);

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
