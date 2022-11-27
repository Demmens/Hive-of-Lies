using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Variable<T> : ScriptableObject
{
    public static implicit operator T(Variable<T> a) => a.Value;

    [SerializeField] private T _value;

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

            return _value; 
        }
        set
        {
            if (!NetworkClient.active && client) return;
            if (!NetworkServer.active && server) return;

            _value = value;
            OnVariableChanged?.Invoke(value);
        } 
    }

    public delegate void VariableChanged(T val);
    public event VariableChanged OnVariableChanged;
}
