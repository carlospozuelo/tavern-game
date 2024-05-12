using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private TextMeshProUGUI textHour, textMinute, textDay;

    [SerializeField]
    private Image weatherImage;

    [SerializeField]
    private Sprite morningSunny, sunny, nightSunny;

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
                SetCurrentDay();
            }

            // Update light color and intensity
            Color color = colorCurve.Evaluate(currentTick / 288f);
            float intensity = daylightCurve.Evaluate(currentTick);

            LightWrapper.UpdateLights(color, intensity);

            SetCurrentHour();
            SetCurrentMinute();

            // Update weather image if necessary
            if (currentTick == 0 || currentTick == 144)
            {
                // 6:00 and 18:00
                weatherImage.sprite = morningSunny;
            } else if (currentTick == 36)
            {
                // 9:00
                weatherImage.sprite = sunny;
            } else if (currentTick == 180)
            {
                // 21:00
                weatherImage.sprite = nightSunny;
            }

            yield return new WaitForSecondsRealtime(secondsPerTick);
            // Advance tick
            currentTick++;

        }
    }

    private void SetCurrentHour()
    {
        textHour.text = (((currentTick / 12) + 6) % 24).ToString("D2");
    }

    private void SetCurrentMinute()
    {
        textMinute.text = ((currentTick % 12) * 5).ToString("D2");
    }

    private void SetCurrentDay()
    {
        textDay.text = "Day " + currentDay.ToString();
    }
}
