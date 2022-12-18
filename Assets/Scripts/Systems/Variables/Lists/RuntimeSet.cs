using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : Variable<List<T>>
{
    public delegate void ListChanged(T item);
    public event ListChanged BeforeItemAdded;
    public event ListChanged AfterItemAdded;

    public event ListChanged BeforeItemRemoved;
    public event ListChanged AfterItemRemoved;

    public void Awake()
    {
        Value = new List<T>();
    }

    public void Add(T item) {
        if (Value.Contains(item)) return;

        BeforeItemAdded?.Invoke(item);
        Value.Add(item);
        AfterItemAdded?.Invoke(item);
    }

    public void Remove(T item)
    {
        if (!Value.Contains(item)) return;

        BeforeItemRemoved?.Invoke(item);
        Value.Remove(item);
        AfterItemRemoved?.Invoke(item);
    }
}
