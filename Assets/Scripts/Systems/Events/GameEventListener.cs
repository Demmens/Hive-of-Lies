using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public List<GameEvent> Events;
    public UnityEvent Response;

    private void OnEnable()
    {
        Events.ForEach(e => e.RegisterListener(this));
    }

    private void OnDisable()
    {
        Events.ForEach(e => e.UnRegisterListener(this));
    }

    public void OnEventInvoked()
    {
        Response.Invoke();
    }
}
