using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GrassController : MonoBehaviour, TimeSubscriber
{
    public void Notify(string text)
    {
        if (text.Equals("New day"))
        {
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
        GameObject g = Instantiate(spawnPrefab, spawnPoints[Random.Range(0, spawnPoints.Count)], Quaternion.identity, parent);

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
