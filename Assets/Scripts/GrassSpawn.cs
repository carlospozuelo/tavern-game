using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GrassSpawn : MonoBehaviour, TimeSubscriber
{

    [SerializeField]
    private List<Sprite> plucks;

    [SerializeField]
    private Material[] windMaterial;

    [SerializeField]
    [Range(1, 18)]
    private int numberOfPlucks = 8;

    private int currentPlucks = 0;


    [SerializeField]
    private GameObject pluckPrefab;

    [SerializeField]
    [Range (.1f, 1.5f)]
    private float spread = .5f;

    private bool generated = false;

    private bool debug = false;

    public void GeneratePlucks(int numberOfPlucks = 0, bool cleans = true)
    {
        if (numberOfPlucks == 0)
        {
            numberOfPlucks = this.numberOfPlucks;
        }

        if (numberOfPlucks > 18) { numberOfPlucks = 18; }

        // Clear current plucks
        if (cleans)
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
            currentPlucks = 0;
        }

        for (int i = currentPlucks; i < numberOfPlucks; i++)
        {
            GameObject pluck = Instantiate(pluckPrefab, transform.position + new Vector3(Random.Range(spread * -1, spread), Random.Range(spread * -1, spread)), Quaternion.identity, transform);

            SpriteRenderer renderer = pluck.GetComponent<SpriteRenderer>();
            renderer.sprite = plucks[Random.Range(0, plucks.Count)];

            renderer.material = windMaterial[Random.Range(0, windMaterial.Length)];
        }

        currentPlucks = numberOfPlucks;

        generated = true;
    }

    public void Notify(string text)
    {
        if (text.Equals("Grow"))
        {
            float chance = .1f;
            if (currentPlucks < 18)
            {
                GeneratePlucks(currentPlucks + Random.Range(4, 8), false);
            } else
            {
                chance += .7f;
            }

            if (Random.Range(0f, 1f) < chance)
            {
                Grow();
            }
        }
    }

    private void Grow()
    {
        // Spread in a random direction (if possible)
        List<Vector2> positions = new List<Vector2>();
        if (AnalyzeDirection(Vector2.up)) { positions.Add(Vector2.up); }
        if (AnalyzeDirection(Vector2.down)) { positions.Add(Vector2.down); }
        if (AnalyzeDirection(Vector2.left)) { positions.Add(Vector2.left); }
        if (AnalyzeDirection(Vector2.right)) { positions.Add(Vector2.right); }

        if (positions.Count > 0)
        {
            Grow(positions[Random.Range(0, positions.Count)]);
        } else
        {
            TimeController.Unsubscribe(this, "Grow");
        }

    }

    private void Grow(Vector2 direction)
    {
        GrassController.SpawnGrass(transform.position + new Vector3(direction.x, direction.y));
    }

    private bool AnalyzeDirection(Vector2 direction)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + new Vector3(direction.x, direction.y), new Vector2(1, 1), 0f);

        foreach (Collider2D collider in colliders) {
            if (collider.gameObject != gameObject) { 
                return false; }
        }

        return true;
    }

    private void Start()
    {
        if (!generated)
        {
            GeneratePlucks();
        }
        GrassController.CountGrass();
        // Subscribe to grow
        TimeController.SubscribeToTick(this, 0, "Grow");
    }

    private void Update()
    {
        // For debugging purposes

        if (debug && currentPlucks != numberOfPlucks)
        {
            GeneratePlucks();
        }
    }
}
