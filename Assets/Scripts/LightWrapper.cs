using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// This class is a wrapper for all the lights that should be affected by the environment changes. So it will affect windows and global light, but not items like fire.
public class LightWrapper : MonoBehaviour
{
    [SerializeField]
    private Light2D light2d;

    private static List<LightWrapper> lights;

    [SerializeField]
    private bool affectsIntensity = false;

    private void Awake()
    {
        if (lights == null)
        {
            lights = new List<LightWrapper>();
        }

        if (!lights.Contains(this)) { 
            lights.Add(this);
        }
    }

    public static void UpdateLights(Color lightColor, float intensity)
    {
        foreach (LightWrapper light in lights)
        {
            if (light.affectsIntensity)
            {
                light.light2d.intensity = intensity;
            }

            light.light2d.color = lightColor;
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
