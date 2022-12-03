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

public class GameEventListener<T> : MonoBehaviour
{
    public List<GameEvent<T>> Events;
    public UnityEvent<T> Response;

    private void OnEnable()
    {
        Events.ForEach(e => e.RegisterListener(this));
    }

    private void OnDisable()
    {
        Events.ForEach(e => e.UnRegisterListener(this));
    }

    public void OnEventInvoked(T arg)
    {
        Response.Invoke(arg);
    }
}

public class GameEventListener<T0,T1> : MonoBehaviour
{
    public List<GameEvent<T0,T1>> Events;
    public UnityEvent<T0,T1> Response;

    private void OnEnable()
    {
        Events.ForEach(e => e.RegisterListener(this));
    }

    private void OnDisable()
    {
        Events.ForEach(e => e.UnRegisterListener(this));
    }

    public void OnEventInvoked(T0 arg0, T1 arg1)
    {
        Response.Invoke(arg0, arg1);
    }
}

public class GameEventListener<T0, T1, T2> : MonoBehaviour
{
    public List<GameEvent<T0, T1, T2>> Events;
    public UnityEvent<T0, T1, T2> Response;

    private void OnEnable()
    {
        Events.ForEach(e => e.RegisterListener(this));
    }

    private void OnDisable()
    {
        Events.ForEach(e => e.UnRegisterListener(this));
    }

    public void OnEventInvoked(T0 arg0, T1 arg1, T2 arg2)
    {
        Response.Invoke(arg0, arg1, arg2);
    }
}

public class GameEventListener<T0, T1, T2, T3> : MonoBehaviour
{
    public List<GameEvent<T0, T1, T2, T3>> Events;
    public UnityEvent<T0, T1, T2, T3> Response;

    private void OnEnable()
    {
        Events.ForEach(e => e.RegisterListener(this));
    }

    private void OnDisable()
    {
        Events.ForEach(e => e.UnRegisterListener(this));
    }

    public void OnEventInvoked(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        Response.Invoke(arg0, arg1, arg2, arg3);
    }
}