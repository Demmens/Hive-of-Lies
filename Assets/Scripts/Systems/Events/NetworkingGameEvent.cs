using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Event", menuName = "Networking Event")]
public class NetworkingGameEvent : ScriptableObject
{

    private List<NetworkingGameEventListener> listeners = new List<NetworkingGameEventListener>();

    public void Invoke(NetworkConnection conn)
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnEventInvoked(conn);
        }
    }

    public void RegisterListener(NetworkingGameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(NetworkingGameEventListener listener)
    {
        listeners.Remove(listener);
    }
}
