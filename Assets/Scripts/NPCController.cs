using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCController : MonoBehaviour, TimeSubscriber
{
    private static NPCController instance;

    private HashSet<Interactuable> interactuablesForNPCS;

    private HashSet<Bench> benchesForNPCS;

    private List<NPC> npcs;

    [SerializeField]
    private int maxNPCS = 5;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("This field determines the chance of spawning an NPC each tick, given that there's NO other npc")]
    private float defaultAttractive = .5f;

    [SerializeField]
    [Range(0.5f, 1f)]
    [Tooltip("This field determines the decay in attractiveness for each npc that is currently in the tavern." +
        "This makes it so that it's harder to spawn npcs the more npcs are currently spawned." +
        "The higher the number, the lesser the decay (Attractive = default attractive * decay^currentnpcs)")]
    private float attractiveDecay = .9f;

    public static void DestroyNPC(NPC npc)
    {
        instance.npcs.Remove(npc);
    }

    [SerializeField]
    private GameObject npcPrefab;
    private bool initialized = false;

    private const string TICK_TEXT = "Tick";

    private void Start()
    {
        if (initialized) return;

        interactuablesForNPCS = new HashSet<Interactuable>();
        benchesForNPCS = new HashSet<Bench>();

        npcs = new List<NPC>();
        TimeController.Subscribe(this, TICK_TEXT, 1, 1, true);

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

        instance.npcs.Add(npc);
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

    // This function is called every tick (every 5 ingame minutes).
    private void HandleNPCS()
    {
        float attractiveness = CalculateAttractiveness();
        int maxNPCS = Mathf.Min(benchesForNPCS.Count, this.maxNPCS);

        if (npcs.Count < maxNPCS)
        {
            if (Random.Range(0f, 1f) < attractiveness)
            {
                // Spawn npc
                print("Spawn " + attractiveness);
                SpawnNPC();
            }
            else
            {
                print("Unlucky " + attractiveness);
            }
        }
    }

    private float CalculateAttractiveness()
    {
        return defaultAttractive * (Mathf.Pow(attractiveDecay, npcs.Count));
    }

    public void Notify(string text)
    {
        if (text.Equals(TICK_TEXT))
        {
            HandleNPCS();
        }
    }
}
