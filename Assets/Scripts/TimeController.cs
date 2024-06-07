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

    private List<TimeSubscriberWrapper> subscribers;

    private Dictionary<int, List<TickSubscriber>> tickSubscribers;


    public static int GetCurrentTick() { return instance.currentTick; }
    public static void Subscribe(TimeSubscriber subscriber, string uniqueName, int notifiesEveryXTicks = 1, int notifyXTimes = 1, bool notifiesForever = false)
    {
        TimeSubscriberWrapper wrapper = new TimeSubscriberWrapper();
        wrapper.timeSubscriber = subscriber;
        wrapper.notifiesForever = notifiesForever;
        wrapper.notifyXTimes = notifyXTimes;
        wrapper.notifyEveryXTicks = notifiesEveryXTicks;

        wrapper.id = uniqueName;

        instance.subscribers.Add(wrapper);
    }

    public static void SubscribeIfNotAlready(TimeSubscriber subscriber, string uniqueName, int notifiesEveryXTicks = 1, int notifyXTimes = 1, bool notifiesForever = false)
    {
        foreach (var wrapper in instance.subscribers)
        {
            if (wrapper.timeSubscriber.Equals(subscriber) && wrapper.id.Equals(uniqueName))
            {
                return;
            }
        }

        Subscribe(subscriber, uniqueName, notifiesEveryXTicks, notifyXTimes, notifiesForever);
    }

    public static void SubscribeToTick(TimeSubscriber subscriber, int tick, string uniqueName, bool forever = false)
    {
        TickSubscriber wrapper = new TickSubscriber();

        wrapper.timeSubscriber = subscriber;
        wrapper.id = uniqueName;
        wrapper.notifyOnTickX = tick;
        wrapper.forever = forever;

        if (!instance.tickSubscribers.ContainsKey(tick))
        {
            instance.tickSubscribers.Add(tick, new List<TickSubscriber>());
        }

        instance.tickSubscribers[tick].Add(wrapper);
    }

    public static void Unsubscribe(TimeSubscriber subscriber, string uniqueName, bool notifiesForever = false)
    {
        for (int i = instance.subscribers.Count - 1; i >= 0; i--)
        {
            var wrapper = instance.subscribers[i];

            if (wrapper != null && wrapper.timeSubscriber.Equals(subscriber) && wrapper.id.Equals(uniqueName))
            {
                instance.subscribers.Remove(wrapper);
                // return;
            }
        }


        foreach (var key in instance.tickSubscribers)
        {
            for (int i = key.Value.Count - 1; i >= 0; i--)
            {
                var wrapper = key.Value[i];

                if (wrapper != null && key.Value[i].Equals(subscriber) && key.Value[i].id.Equals(uniqueName))
                {
                    key.Value.Remove(wrapper);
                }
            }
        }

    }

    public static void Unsubscribe(TimeSubscriber subscriber)
    {
        for (int i = instance.subscribers.Count - 1; i >= 0; i--) 
        {
            var wrapper = instance.subscribers[i];

            if (wrapper != null && wrapper.timeSubscriber.Equals(subscriber))
            {
                instance.subscribers.Remove(wrapper);
            }
        }

        foreach (var key in instance.tickSubscribers)
        {
            for (int i = key.Value.Count - 1; i >= 0; i--)
            {
                var wrapper = key.Value[i];

                if (wrapper != null && key.Value[i].Equals(subscriber))
                {
                    key.Value.Remove(wrapper);
                }
            }
        }

    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Initialize();
        }
    }

    private bool initialized = false;
    private void Initialize()
    {
        if (initialized) return;

        instance = this;
        subscribers = new List<TimeSubscriberWrapper>();
        tickSubscribers = new Dictionary<int, List<TickSubscriber>>();
        StartCoroutine(TimeCoroutine());

        initialized = true;

    }


    private IEnumerator TimeCoroutine()
    {
        while (true)
        {
            // Notify all listeners of a new tick
            for (int i = subscribers.Count - 1; i >= 0; i--)
            {
                TimeSubscriberWrapper wrapper = subscribers[i];
                wrapper.ticksElapsed++;

                if (wrapper.ticksElapsed >= wrapper.notifyEveryXTicks)
                {
                    wrapper.ticksElapsed = 0;
                    // Notify
                    wrapper.timeSubscriber.Notify(wrapper.id);
                    if (!wrapper.notifiesForever) { wrapper.timesNotified++; }
                }

                if (!wrapper.notifiesForever && wrapper.timesNotified >= wrapper.notifyXTimes)
                {
                    // Notification expired. Remove from the dictionary
                    subscribers.Remove(wrapper);
                }
            }

            // Notify all listeners of the tick
            if (tickSubscribers.ContainsKey(currentTick))
            {
                foreach (TickSubscriber sub in tickSubscribers[currentTick])
                {
                    sub.timeSubscriber.Notify(sub.id);
                }
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

            yield return new WaitForSeconds(secondsPerTick);
            // Advance tick
            currentTick++;

            if (currentTick >= ticksPerDay)
            {
                // Next day
                currentTick = 0;
                currentDay++;
                SetCurrentDay();
            }

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
