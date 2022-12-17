using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class NetworkingGameEventListener : MonoBehaviour
{
    public List<NetworkingGameEvent> Events;
    public UnityEvent<NetworkConnection> Response;

    private void OnEnable()
    {
        Events.ForEach(e => e.RegisterListener(this));
    }

    private void OnDisable()
    {
        Events.ForEach(e => e.UnRegisterListener(this));
    }

    public void OnEventInvoked(NetworkConnection conn)
    {
        Response.Invoke(conn);
    }
}
