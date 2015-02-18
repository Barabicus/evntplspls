using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameEvent : MonoBehaviour
{
    [HideInInspector]
    public List<GameEventTrigger> triggers;
    public bool parentIsTrigger = true;

    public virtual void Awake() { }
    public virtual void Start()
    {
        if (parentIsTrigger)
        {
            if (transform.parent != null)
            {
                GameEventTrigger ownerTrigger = transform.parent.GetComponent<GameEventTrigger>();
                if (ownerTrigger != null)
                    triggers.Add(ownerTrigger);
            }
        }

        foreach (GameEventTrigger tr in triggers)
        {
            tr.onTriggerEnter += TriggerEnterEvent;
            tr.onTriggerExit += TriggerExitEvent;
        }

    }
    public virtual void TriggerEnterEvent(Collider other) { }
    public virtual void TriggerExitEvent(Collider other) { }

    public virtual void Update() { }



}
