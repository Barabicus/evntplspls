using UnityEngine;
using System.Collections;

public class TimedTrigger : GameEventTrigger
{
    public float triggerTimeForward = 1f;
    public float triggerTimeReversed = 1f;

    private Timer timer;
    private bool isForward = true;

    public override void Start()
    {
        base.Start();
        timer = new Timer(triggerTimeForward);
    }

    private void Update()
    {
        if (timer.CanTick)
        {
            if (isForward)
                TriggerEnter(null);
            else
                TriggerExit(null);

            isForward = !isForward;
            timer = isForward ? new Timer(triggerTimeForward) : new Timer(triggerTimeReversed);
        }
    }


}
