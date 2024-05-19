using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TimeSubscriber 
{

    void Notify(string text);
}

public class TimeSubscriberWrapper
{
    public string id;

    public TimeSubscriber timeSubscriber;
    public int notifyEveryXTicks, notifyXTimes, ticksElapsed, timesNotified;

    public bool notifiesForever;
}
