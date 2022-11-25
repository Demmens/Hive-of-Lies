using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Variable<T> : ScriptableObject
{
    private T val;

    /// <summary>
    /// Is this variable accessible on clients
    /// </summary>
    [SerializeField] private bool server;
    /// <summary>
    /// Is this variable accessible on the server
    /// </summary>
    [SerializeField] private bool client;

    /// <summary>
    /// The value of this variable
    /// </summary>
    public T Value 
    { 
        get 
        {
            if (!NetworkClient.active && client) Debug.LogError($"Tried to get value of {name} from a client, but it is not a client variable.");
            if (!NetworkServer.active && server) Debug.LogError($"Tried to get value of {name} from a server, but it is not a server variable.");

            return val; 
        }
        set
        {
            if (!NetworkClient.active && client) return;
            if (!NetworkServer.active && server) return;

            val = value;
            OnVariableChanged?.Invoke(value);
        } 
    }

    public delegate void VariableChanged(T val);
    public event VariableChanged OnVariableChanged;
}
