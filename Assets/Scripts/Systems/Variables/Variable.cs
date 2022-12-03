using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Variable<T> : ScriptableObject
{
    public static implicit operator T(Variable<T> a) => a.Value;

    [Tooltip("The initial value of this variable")]
    [SerializeField] private T initialValue;

    [Tooltip("The current value of this variable")]
    [SerializeField] private T currentValue;

    [Space]
    [Space]

    [Tooltip("Should this variable be accessible on the server")]
    [SerializeField] private bool server;

    [Tooltip("Should this variable be accessible on clients")]
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

            return currentValue; 
        }
        set
        {
            if (!NetworkClient.active && client) return;
            if (!NetworkServer.active && server) return;

            OnVariableChanged?.Invoke(currentValue, ref value);

            currentValue = value;
        } 
    }

    public delegate void VariableChanged(T oldVal, ref T newVal);
    public event VariableChanged OnVariableChanged;

    private void OnEnable()
    {
        //Set _value to bypass all the code that runs from setting Value
        currentValue = initialValue;
    }
}
