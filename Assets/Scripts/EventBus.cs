using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

    public static void Subscribe<T>(Action<T> callback)
    {
        if (callback == null) 
        { 
            return;
        }

        Type eventType = typeof(T);

        if (_events.TryGetValue(eventType, out Delegate eventDelegate))
        {
            _events[eventType] = Delegate.Combine(eventDelegate, callback);
        }
        else
        {
            _events[eventType] = callback;
        }
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (callback == null) 
        {
            return;
        }

        Type eventType = typeof(T);

        if (_events.TryGetValue(eventType, out Delegate eventDelegate))
        {
            Delegate targetDelegate = Delegate.Remove(eventDelegate, callback);

            if (targetDelegate == null)
            {
                _events.Remove(eventType);
            }
            else
            {
                _events[eventType] = targetDelegate;
            }
        }
    }

    public static void Invoke<T>(T eventData)
    {
        Type eventType = typeof(T);

        if (_events.TryGetValue(eventType, out Delegate eventDelegate))
        {
            Action<T> action = eventDelegate as Action<T>;

            action?.Invoke(eventData);
        }
    }

    public static void Clear()
    {
        _events.Clear();
    }
}