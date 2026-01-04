using System;
using System.Collections.Generic;
using System.Diagnostics;

public class EventManager : Singleton<EventManager>
{
    // Delegate로 변경하여 모든 타입의 EventHandler 지원
    private readonly Dictionary<string, Delegate> eventHandlerDic = new();

    // EventHandler 추가 (+= 동작)
    public void AddEventListner(string event_name, EventHandler event_handler)
    {
        if (!eventHandlerDic.ContainsKey(event_name))
            eventHandlerDic[event_name] = event_handler;
        else
            eventHandlerDic[event_name] = Delegate.Combine(eventHandlerDic[event_name], event_handler);
    }

    // EventHandler<T> 추가 (+= 동작)
    public void AddEventListner<T>(string event_name, EventHandler<T> event_handler) where T : EventArgs
    {
        if (!eventHandlerDic.ContainsKey(event_name))
            eventHandlerDic[event_name] = event_handler;
        else
            eventHandlerDic[event_name] = Delegate.Combine(eventHandlerDic[event_name], event_handler);
    }

    // EventHandler 제거 (-= 동작)
    public void RemoveEventListner(string event_name, EventHandler event_handler)
    {
        if (!eventHandlerDic.ContainsKey(event_name))
            return;

        eventHandlerDic[event_name] = Delegate.Remove(eventHandlerDic[event_name], event_handler);

        // 모두 제거되면 딕셔너리에서도 제거
        if (eventHandlerDic[event_name] == null)
            eventHandlerDic.Remove(event_name);
    }

    // EventHandler<T> 제거 (-= 동작)
    public void RemoveEventListner<T>(string event_name, EventHandler<T> event_handler) where T : EventArgs
    {
        if (!eventHandlerDic.ContainsKey(event_name))
            return;

        eventHandlerDic[event_name] = Delegate.Remove(eventHandlerDic[event_name], event_handler);

        // 모두 제거되면 딕셔너리에서도 제거
        if (eventHandlerDic[event_name] == null)
            eventHandlerDic.Remove(event_name);
    }

    // 모든 리스너 제거
    public void RemoveEventListners(string event_name)
    {
        if (eventHandlerDic.ContainsKey(event_name))
            eventHandlerDic[event_name] = null;
    }

    // Trigger - EventHandler (EventArgs 없음)
    public void Trigger(string event_name, object sender)
    {
        if (eventHandlerDic.ContainsKey(event_name))
            (eventHandlerDic[event_name] as EventHandler)?.Invoke(sender, EventArgs.Empty);
    }

    // Trigger - EventHandler (EventArgs 있음)
    public void Trigger(string event_name, object sender, EventArgs args)
    {
        if (eventHandlerDic.ContainsKey(event_name))
            (eventHandlerDic[event_name] as EventHandler)?.Invoke(sender, args);
    }

    // Trigger - EventHandler<T> (커스텀 EventArgs)
    public void Trigger<T>(string event_name, object sender, T args) where T : EventArgs
    {
        if (eventHandlerDic.ContainsKey(event_name))
            (eventHandlerDic[event_name] as EventHandler<T>)?.Invoke(sender, args);
    }

    public void Clear(string event_name)
    {
        if (eventHandlerDic.ContainsKey(event_name))
            eventHandlerDic[event_name] = null;
    }

    public void AllClear()
    {
        eventHandlerDic.Clear();
    }
}
