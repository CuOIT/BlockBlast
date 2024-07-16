using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event<T> : ScriptableObject
{
    
    public List<EventListener<T>> listeners = new List<EventListener<T>>();


    public void RaiseEvent(T t)
    {
        foreach(var listener in listeners)
        {
            listener.OnEvent(t);
        }
    }
    public void AddListener(EventListener<T> listener,bool priority = false )
    {
        if (priority)
        {
            listeners.Insert(0, listener);
        }
        else
        {
            listeners.Add(listener);
        }
      
    }
    public void RemoveListener(EventListener<T> listener)
    {
        listeners.Remove(listener);
    }

    public void RemoveAllListener()
    {
        listeners.Clear();
    }
    private void OnDisable()
    {
        RemoveAllListener();
    }
}

[CreateAssetMenu(fileName ="Event",menuName ="Event/Simple")]
public class Event : ScriptableObject
{

    public List<EventListener> listeners = new List<EventListener>();
    public void RaiseEvent()
    {
        foreach (var listener in listeners)
        {
            listener?.OnEvent();
        }
    }
    public void AddListener(EventListener listener,bool priority=false)
    {
        if (priority)
        {
            listeners.Insert(0, listener);
        }
        else
        {
            listeners.Add(listener);
        }
    }
    public void RemoveListener(EventListener listener)
    {
        listeners.Remove(listener);
    }

    public void RemoveAllListener()
    {
        listeners.Clear();
    }
    private void OnDisable()
    {
        RemoveAllListener();
    }
}
