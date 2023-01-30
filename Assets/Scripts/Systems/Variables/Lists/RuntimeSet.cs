using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class RuntimeSet<T> : ScriptableObject
{
    public static implicit operator List<T>(RuntimeSet<T> a) => a.Value;

    [Tooltip("The initial value of this variable")]
    [SerializeField] private List<T> initialValue;

    [Tooltip("The current value of this variable")]
    [SerializeField] private List<T> currentValue;

    [Space]
    [Space]

    [Tooltip("Should this variable be accessible on the server")]
    [SerializeField] private bool server;

    [Tooltip("Should this variable be accessible on clients")]
    [SerializeField] private bool client;

    [Tooltip("Whether this variable should persist through scene changes")]
    public bool Persistent = true;

    [Space]
    [Space]

    [TextArea]
    [Tooltip("The tooltip for this variable. Doesn't have any mechanical purpose.")]
    [SerializeField] private string tooltip;

    /// <summary>
    /// The value of this variable
    /// </summary>
    public List<T> Value
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

            currentValue = value;
        }
    }

    public delegate void ListChanged(T item);
    public event ListChanged BeforeItemAdded;
    public event ListChanged AfterItemAdded;

    public event ListChanged BeforeItemRemoved;
    public event ListChanged AfterItemRemoved;

    public void Add(T item) {
        if (Value.Contains(item)) return;

        BeforeItemAdded?.Invoke(item);
        currentValue.Add(item);
        AfterItemAdded?.Invoke(item);
    }

    public void Remove(T item)
    {
        if (!Value.Contains(item)) return;

        BeforeItemRemoved?.Invoke(item);
        currentValue.Remove(item);
        AfterItemRemoved?.Invoke(item);
    }

    public void OnEnable()
    {
        BeforeItemAdded = null;
        AfterItemAdded = null;
        BeforeItemRemoved = null;
        AfterItemRemoved = null;
        OnValidate();
    }

    private void OnValidate()
    {
        //Set currentValue to bypass all the code that runs from setting Value
        //For some reason this isn't the same as just setting the current value to the initial value..
        currentValue = new();
        if (initialValue != null) currentValue.AddRange(initialValue);
    }
}
