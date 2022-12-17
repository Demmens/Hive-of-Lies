using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "Events/Basic")]
public class GameEvent : ScriptableObject
{

    private List<GameEventListener> listeners = new List<GameEventListener>();

    public void Invoke()
    {
        for (int i = 0; i < listeners.Count; i++)
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
        for (int i = 0; i < listeners.Count; i++)
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
