using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "Event")]
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
