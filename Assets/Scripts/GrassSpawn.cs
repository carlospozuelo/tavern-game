using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpawn : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> plucks;

    [SerializeField]
    private Material windMaterial;

    [SerializeField]
    [Range(1, 24)]
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
        print("Generating");
        if (numberOfPlucks == 0)
        {
            numberOfPlucks = this.numberOfPlucks;
        }
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

            renderer.material = new Material(windMaterial);
            renderer.material.SetFloat("WindScale", 1 + Random.Range(-0.2f, 0.2f));
        }

        currentPlucks = numberOfPlucks;

        generated = true;
    }

    private void Start()
    {
        if (!generated)
        {
            GeneratePlucks();
        }
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
