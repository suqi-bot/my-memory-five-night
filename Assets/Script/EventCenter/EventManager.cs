using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfoBase { }

public class EventInfo0 : IEventInfoBase
{
    public UnityAction actions;
    public EventInfo0(UnityAction action) { actions += action; }
}

public class EventInfo1<T> : IEventInfoBase
{
    public UnityAction<T> actions;
    public EventInfo1(UnityAction<T> action) { actions += action; }
}

public class EventInfo2<T1, T2> : IEventInfoBase
{
    public UnityAction<T1, T2> actions;
    public EventInfo2(UnityAction<T1, T2> action) { actions += action; }
}

public class EventInfo3<T1, T2, T3> : IEventInfoBase
{
    public UnityAction<T1, T2, T3> actions;
    public EventInfo3(UnityAction<T1, T2, T3> action) { actions += action; }
}

public class EventManager : MonoSingleton<EventManager>
{
    private Dictionary<EventType, IEventInfoBase> eventDic = new Dictionary<EventType, IEventInfoBase>();

    public void AddEventListener(EventType eventType, UnityAction action)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo0).actions += action;
        else
            eventDic.Add(eventType, new EventInfo0(action));
    }

    public void AddEventListener<T>(EventType eventType, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo1<T>).actions += action;
        else
            eventDic.Add(eventType, new EventInfo1<T>(action));
    }

    public void AddEventListener<T1, T2>(EventType eventType, UnityAction<T1, T2> action)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo2<T1, T2>).actions += action;
        else
            eventDic.Add(eventType, new EventInfo2<T1, T2>(action));
    }

    public void AddEventListener<T1, T2, T3>(EventType eventType, UnityAction<T1, T2, T3> action)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo3<T1, T2, T3>).actions += action;
        else
            eventDic.Add(eventType, new EventInfo3<T1, T2, T3>(action));
    }

    public void RemoveEventListener(EventType eventType, UnityAction action)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo0).actions -= action;
    }

    public void RemoveEventListener<T>(EventType eventType, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo1<T>).actions -= action;
    }

    public void RemoveEventListener<T1, T2>(EventType eventType, UnityAction<T1, T2> action)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo2<T1, T2>).actions -= action;
    }

    public void RemoveEventListener<T1, T2, T3>(EventType eventType, UnityAction<T1, T2, T3> action)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo3<T1, T2, T3>).actions -= action;
    }

    public void EventTrigger(EventType eventType)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo0)?.actions?.Invoke();
    }

    public void EventTrigger<T>(EventType eventType, T arg)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo1<T>)?.actions?.Invoke(arg);
    }

    public void EventTrigger<T1, T2>(EventType eventType, T1 arg1, T2 arg2)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo2<T1, T2>)?.actions?.Invoke(arg1, arg2);
    }

    public void EventTrigger<T1, T2, T3>(EventType eventType, T1 arg1, T2 arg2, T3 arg3)
    {
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo3<T1, T2, T3>)?.actions?.Invoke(arg1, arg2, arg3);
    }

    public void ClearAllEventListener()
    {
        eventDic.Clear();
    }
}
