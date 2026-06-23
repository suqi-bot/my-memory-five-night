using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
//改为UTF-8编码
public interface IEventInfo { }
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> actions)
    {
        this.actions += actions;
    }
}
public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public EventInfo(UnityAction actions)
    {
        this.actions = actions;
    }
}
public class EventCenter : BaseManager<EventCenter>
{

    private Dictionary<E_EventType, IEventInfo> eventDic = new Dictionary<E_EventType, IEventInfo>();

    public void AddEventListener<T>(E_EventType eventType, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(eventType))
        {
            (eventDic[eventType] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(eventType,new EventInfo<T>(action));
        }
    }
    public void AddEventListener(E_EventType eventType, UnityAction action)
    {
        if (eventDic.ContainsKey(eventType))
        {
            (eventDic[eventType] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(eventType,new EventInfo(action));
        }
    }
    public void RemoveEventListener<T>(E_EventType eventType, UnityAction<T> action = null)
    {
        if(eventDic.ContainsKey(eventType))
        {
            if(action != null)(eventDic[eventType] as EventInfo<T>).actions -= action;
            else
            {
                (eventDic[eventType] as EventInfo<T>).actions = null;
            }
        }
    }
    public void RemoveEventListener(E_EventType eventType, UnityAction action = null)
    {
        if(eventDic.ContainsKey(eventType))
        {
            if(action != null)(eventDic[eventType] as EventInfo).actions -= action;
            else
            {
                (eventDic[eventType] as EventInfo).actions = null;
            }
        }
    }
    public void ClearAllEventListener()
    {
        eventDic.Clear();
    }
    public void EventTrigger<T>(E_EventType eventType,T info)
    {
        if (eventDic.ContainsKey(eventType))
        {
            if ((eventDic[eventType] as EventInfo<T>).actions != null)
                (eventDic[eventType] as EventInfo<T>).actions.Invoke(info);
        }
    }
    public void EventTrigger(E_EventType eventType)
    {
        if (eventDic.ContainsKey(eventType))
        {
            if((eventDic[eventType] as EventInfo).actions != null)
                (eventDic[eventType] as EventInfo).actions.Invoke();
        }
    }

}
/// <summary>
/// 事件类型枚举
/// </summary>
public enum E_EventType
{
    
}
