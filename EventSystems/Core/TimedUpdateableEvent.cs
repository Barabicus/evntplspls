using UnityEngine;
using System.Collections;
using System;

public abstract class TimedUpdateableEvent : GameEvent
{
    public float updateTime = 1f;
  //  public AnimationCurve timeCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    private float _currentTime = 0f;
    private float _updateDirection = 0f;
    /// <summary>
    /// The update percent amount from 0 - 1
    /// </summary>
    public float UpdatePercent
    {
        get { return  (_currentTime / updateTime) /* timeCurve.Evaluate(_currentTime / updateTime) */; }
    }

    public event Action eventFinished;

    private Timer timer;


    public override void TriggerEnterEvent(Collider other)
    {
        base.TriggerEnterEvent(other);
        _updateDirection = 1f;
    }

    public override void TriggerExitEvent(Collider other)
    {
        base.TriggerExitEvent(other);
        _updateDirection = -1f;
    }

    public override void Update()
    {
        base.Update();
        _currentTime = Mathf.Clamp(_currentTime + (Time.deltaTime * _updateDirection), 0f, updateTime);
    }

}
