using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Reflection;
using System.Collections.Generic;

public class GeneralEvent : TimedUpdateableEvent
{
    #region Fields
    [Tooltip("Modify the target value over time. The X axis represents time as a percent from 0 to 1 and the Y axis represents the multiplication amount")]
    public AnimationCurve valueMultiplierCurve = AnimationCurve.Linear(0, 1f, 1f, 1f);

    [HideInInspector]
    public List<EventWrapper> eventsList = new List<EventWrapper>();
    private List<EventInfo> eventsInfo = new List<EventInfo>();

    public delegate object UpdateAction(EventInfo info);
    #endregion

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < eventsList.Count; i++)
        {
            try
            {
                if (eventsList[i].targetComponent == null || eventsList[i].memberName.Equals(""))
                    return;

                // Create the associated Event Info

                MemberInfo memInfo = eventsList[i].targetComponent.GetType().GetMember(eventsList[i].memberName)[0];
                UpdateAction updateAction = GetUpdateAction(eventsList[i]);
                object startValue = null;

                if (memInfo is FieldInfo)
                    startValue = ((FieldInfo)memInfo).GetValue(eventsList[i].targetComponent);
                else if (memInfo is PropertyInfo)
                    startValue = ((PropertyInfo)memInfo).GetValue(eventsList[i].targetComponent, null);

                if (startValue == null)
                {
                    Debug.LogError("Start value was null for event: " + gameObject.name);
                    enabled = false;
                    return;
                }

                eventsInfo.Add(new EventInfo(memInfo, updateAction, startValue, eventsList[i].targetComponent, eventsList[i].targetValue));

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

    }

    private UpdateAction GetUpdateAction(EventWrapper eventInfo)
    {
        if (eventInfo.targetType == typeof(float).ToString())
            return UpdateFloatValue;
        else if (eventInfo.targetType == typeof(Color).ToString())
            return UpdateColorValue;
        else if (eventInfo.targetType == typeof(Vector3).ToString())
            return UpateVector3Value;
        else
            return null;
    }

    public override void TriggerEnterEvent(Collider other)
    {
        base.TriggerEnterEvent(other);
    }

    public override void Update()
    {
        base.Update();

        foreach (EventInfo eventInfo in eventsInfo)
        {
            if (eventInfo.updateAction != null)
            {
                object value = eventInfo.updateAction(eventInfo);
                if (eventInfo.memInfo is FieldInfo)
                    HandleFieldInfo((FieldInfo)eventInfo.memInfo, eventInfo.targetComponent, value);
                else if (eventInfo.memInfo is PropertyInfo)
                    HandlePropertyInfo((PropertyInfo)eventInfo.memInfo, eventInfo.targetComponent, value);
            }

        }
    }

    #region Member Handling

    private void HandleFieldInfo(FieldInfo info, Component targetComponent, object value)
    {
        info.SetValue(targetComponent, value);
    }

    private void HandlePropertyInfo(PropertyInfo info, Component targetComponent, object value)
    {
        info.SetValue(targetComponent, value, null);
    }

    #endregion

    #region Type Updating

    private object UpdateFloatValue(EventInfo info)
    {
        return Mathf.Lerp((float)info.startValue, info.targetValue.floatValue * valueMultiplierCurve.Evaluate(UpdatePercent), UpdatePercent);
    }

    private object UpdateColorValue(EventInfo info)
    {
        return Color.Lerp((Color)info.startValue, info.targetValue.colorValue * valueMultiplierCurve.Evaluate(UpdatePercent), UpdatePercent);
    }

    private object UpateVector3Value(EventInfo info)
    {
        return Vector3.Lerp((Vector3)info.startValue, info.targetValue.vector3Value * valueMultiplierCurve.Evaluate(UpdatePercent), UpdatePercent);
    }

    #endregion

    #region Member List



    #endregion



    [System.Serializable]
    public struct TargetValueWrapper
    {
        public Vector3 vector3Value;
        public Color colorValue;
        public float floatValue;
        public int intValue;
    }

    /// <summary>
    /// Struct detailing variables relating to constructing event information
    /// </summary>
    [System.Serializable]
    public struct EventWrapper
    {
        public GameObject targetObj;
        public Component targetComponent;
        public string memberName;
        public string targetType;
        public TargetValueWrapper targetValue;

        public EventWrapper(GameObject targetObj, Component targetComponent, string memberName, string targetType, TargetValueWrapper targetValue)
        {
            this.targetObj = targetObj;
            this.targetComponent = targetComponent;
            this.memberName = memberName;
            this.targetType = targetType;
            this.targetValue = targetValue;
        }
    }

    /// <summary>
    /// Struct detailing variables relating to event execution
    /// </summary>
    public struct EventInfo
    {
        public MemberInfo memInfo;
        public UpdateAction updateAction;
        public object startValue;
        public Component targetComponent;
        public TargetValueWrapper targetValue;

        public EventInfo(MemberInfo memInfo, UpdateAction updateAction, object startValue, Component targetComponent, TargetValueWrapper targetValue)
        {
            this.memInfo = memInfo;
            this.updateAction = updateAction;
            this.startValue = startValue;
            this.targetComponent = targetComponent;
            this.targetValue = targetValue;
        }

    }

}