using UnityEngine;
using System.Collections;

public struct Timer
{
    private float tickTime;
    private float currentTime;


    public bool CanTick
    {
        get
        {
            if (Time.time - currentTime >= tickTime)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Check to see if the timer has triggered a tick.
    /// If it can tick it will return true and reset the timer
    /// </summary>
    public bool CanTickAndReset()
    {
        if (Time.time - currentTime >= tickTime)
        {
            currentTime = Time.time;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        currentTime = Time.time;
    }

    public Timer(float tickTime)
    {
        this.tickTime = tickTime;
        currentTime = Time.time;
    }

}
