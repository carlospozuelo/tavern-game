using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GrassController : MonoBehaviour, TimeSubscriber
{

    private static GrassController instance;

    [SerializeField]
    private int maxGrass = 400, currentGrass = 0;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else
        {
            instance = this;
        }
    }

    public static void CountGrass() { instance.currentGrass++; }

    public void Notify(string text)
    {
        if (text.Equals("New day"))
        {
            SpawnGrass();
            SpawnGrass();
            SpawnGrass();
            SpawnGrass();
            SpawnGrass();
        }
    }


    [SerializeField]
    private bool debug;

    [SerializeField]
    private List<Vector2> spawnPoints;

    private void OnDrawGizmosSelected()
    {
        if (!debug) { return; }

        Gizmos.color = Color.yellow;

        foreach (Vector2 v in spawnPoints)
        {
            Gizmos.DrawWireSphere(v, 1f);
        }
        
    }


    [SerializeField]
    private GameObject spawnPrefab;

    [SerializeField]
    private Transform parent;

    private void SpawnGrass()
    {
        Vector2 v = spawnPoints[Random.Range(0, spawnPoints.Count)];
        SpawnGrass(v);
    }

    public static void SpawnGrass(Vector2 pos)
    {
        if (instance.currentGrass >= instance.maxGrass)
        {
            Debug.LogWarning("Maximum grass on the map reached");
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, new Vector2(1, 1), 0f);
        if (colliders.Length > 0)
        {
            return;
        }

        GameObject g = Instantiate(instance.spawnPrefab, new Vector3(pos.x, pos.y, instance.spawnPrefab.transform.position.z), Quaternion.identity, instance.parent);

        g.GetComponent<GrassSpawn>().GeneratePlucks(4, true);
    }



    // Start is called before the first frame update
    void Start()
    {
        TimeController.SubscribeToTick(this, 0, "New day", true);
    }

    [MenuItem("Tools/Generate/Grass Controller Initial Vector")]
    static void GenerateVector()
    {
        GrassController g = FindObjectOfType<GrassController>();

        g.spawnPoints = new List<Vector2>();

        for (int i = 0; i <= 13; i++)
        {
            for (int j = 0; j <= 19; j++)
            {
                g.spawnPoints.Add(new Vector2(-30 + (5 * i), -50 + (5*j)));
            }
        }
    }
}
