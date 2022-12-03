using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent : ScriptableObject
{

    private List<GameEventListener> listeners = new List<GameEventListener>();

    public void Invoke()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventInvoked();
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}

public abstract class GameEvent<T> : ScriptableObject
{

    private List<GameEventListener<T>> listeners = new List<GameEventListener<T>>();

    public void Invoke(T arg)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventInvoked(arg);
        }
    }

    public void RegisterListener(GameEventListener<T> listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(GameEventListener<T> listener)
    {
        listeners.Remove(listener);
    }
}

public abstract class GameEvent<T0,T1> : ScriptableObject
{

    private List<GameEventListener<T0,T1>> listeners = new List<GameEventListener<T0,T1>>();

    public void Invoke(T0 arg0, T1 arg1)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventInvoked(arg0, arg1);
        }
    }

    public void RegisterListener(GameEventListener<T0,T1> listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(GameEventListener<T0,T1> listener)
    {
        listeners.Remove(listener);
    }
}

public abstract class GameEvent<T0, T1, T2> : ScriptableObject
{

    private List<GameEventListener<T0, T1, T2>> listeners = new List<GameEventListener<T0, T1, T2>>();

    public void Invoke(T0 arg0, T1 arg1, T2 arg2)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventInvoked(arg0, arg1, arg2);
        }
    }

    public void RegisterListener(GameEventListener<T0, T1, T2> listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(GameEventListener<T0, T1, T2> listener)
    {
        listeners.Remove(listener);
    }
}

public abstract class GameEvent<T0, T1, T2, T3> : ScriptableObject
{

    private List<GameEventListener<T0, T1, T2, T3>> listeners = new List<GameEventListener<T0, T1, T2, T3>>();

    public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventInvoked(arg0, arg1, arg2, arg3);
        }
    }

    public void RegisterListener(GameEventListener<T0, T1, T2, T3> listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(GameEventListener<T0, T1, T2, T3> listener)
    {
        listeners.Remove(listener);
    }
}