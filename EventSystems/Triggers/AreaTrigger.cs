using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaTrigger : GameEventTrigger
{
    [HideInInspector]
    public List<GameObject> triggerableObjects = new List<GameObject>();
    public bool triggerEnterEvents = true;
    public bool triggerExitEvents = true;
    public TriggeredBy triggeredBy = TriggeredBy.All;
    public LayerMask layerMask = -1;

    public enum TriggeredBy
    {
        All,
        Select
    }

    public override void Start()
    {
        base.Start();
    }

    public bool CanTrigger(int layer)
    {
        return ((layerMask & (1 << layer)) > 0);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!CanTrigger(other.gameObject.layer) || !triggerEnterEvents)
            return;

        switch (triggeredBy)
        {
            case TriggeredBy.All:
                TriggerEnter(other);
                break;
            case TriggeredBy.Select:
                foreach (GameObject o in triggerableObjects)
                {
                    if (o != null && o.gameObject == other.gameObject)
                    {
                        TriggerEnter(other);
                        return;
                    }
                }
                break;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!CanTrigger(other.gameObject.layer) || !triggerExitEvents)
            return;

        switch (triggeredBy)
        {
            case TriggeredBy.All:
                TriggerExit(other);
                break;
            case TriggeredBy.Select:
                foreach (GameObject o in triggerableObjects)
                {
                    if (o != null && o.gameObject == other.gameObject)
                    {
                        TriggerExit(other);
                        return;
                    }
                }
                break;
        }
    }


}
