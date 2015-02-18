using UnityEngine;
using System.Collections;
using System;

public abstract class GameEventTrigger : MonoBehaviour
{
    public event Action<Collider> onTriggerEnter;
    public event Action<Collider> onTriggerExit;
    public bool singleFire = false;
    public float triggerDelay = 0f;

    private float _lastTrigger;

    public virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }
    public virtual void Awake() { }
    
    public void TriggerEnter(Collider other)
    {
        if (!singleFire && Time.time - _lastTrigger <= triggerDelay)
            return;

        if (onTriggerEnter != null)
            onTriggerEnter(other);

        if (singleFire)
            Destroy(this);
        else
            _lastTrigger = Time.time;
    }

    public void TriggerExit(Collider other)
    {
        if (!singleFire && Time.time - _lastTrigger <= triggerDelay)
            return;

        if (onTriggerExit != null)
            onTriggerExit(other);

        if (singleFire)
            Destroy(this);
        else
            _lastTrigger = Time.time;
    }
}
