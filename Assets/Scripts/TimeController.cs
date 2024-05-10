using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Each tick represents 5 minutes in game. So a day is 288 ticks")]
    private float secondsPerTick = 4f;

    private int ticksPerDay = 288;

    [SerializeField]
    private int currentDay, currentTick;

    private static TimeController instance;

    [SerializeField]
    private AnimationCurve daylightCurve;

    [SerializeField]
    private Gradient colorCurve;

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

    private void Start()
    {
        StartCoroutine(TimeCoroutine());
    }

    private IEnumerator TimeCoroutine()
    {
        while (true)
        {
            // Notify all listeners of a new tick

            if (currentTick >= ticksPerDay)
            {
                // Next day
                currentTick = 0;
                currentDay++;
            }

            // Update light color and intensity
            Color color = colorCurve.Evaluate(currentTick / 288f);
            float intensity = daylightCurve.Evaluate(currentTick);

            LightWrapper.UpdateLights(color, intensity);

            DebugPanelUI.instance.Debug(GetCurrentHour().ToString("D2") + ":" + GetCurrentMinute().ToString("D2"));
            yield return new WaitForSecondsRealtime(secondsPerTick);
            // Advance tick
            currentTick++;

        }
    }

    private int GetCurrentHour()
    {
        return ((currentTick / 12) + 6) % 24;
    }

    private int GetCurrentMinute()
    {
        return (currentTick % 12) * 5;
    }
}
