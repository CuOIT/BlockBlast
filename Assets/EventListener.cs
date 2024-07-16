using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    [SerializeField] Event _event;

    [SerializeField] bool priorty;

    [SerializeField] UnityEvent unityEvent;

    public void OnEnable()
    {
        _event.AddListener(this,priorty);
    }
    public void OnEvent()
    {
        unityEvent?.Invoke();
    }
    private void OnDisable()
    {
        _event.RemoveListener(this);
    }
}
public class EventListener<T> : MonoBehaviour
{
    [SerializeField] Event<T> _event;
    [SerializeField] bool priority;
    [SerializeField] UnityEvent<T> unityEvent;

    public void OnEnable()
    {
        _event.AddListener(this,priority);
    }
    public void OnEvent(T t )
    {
        unityEvent?.Invoke(t);
    }

    private void OnDisable()
    {
        _event.RemoveListener(this);
    }
}
