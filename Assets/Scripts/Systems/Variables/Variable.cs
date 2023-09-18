using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

public abstract class Variable<T> : ScriptableObject
{
    public static implicit operator T(Variable<T> a) => a.Value;

    [Tooltip("The initial value of this variable")]
    [SerializeField] protected T initialValue;

    [Tooltip("The current value of this variable")]
    [SerializeField] private T currentValue;

    [Tooltip("Whether this variable should persist through scene changes")]
    public bool Persistent = true;

    [Space]
    [Space]

    [Tooltip("Should this variable be accessible on the server")]
    [SerializeField] private bool server;

    [Tooltip("Should this variable be accessible on clients")]
    [SerializeField] private bool client;

#if UNITY_EDITOR
    [Space]
    [Space]

    [TextArea]
    [Tooltip("The tooltip for this variable. Doesn't have any mechanical purpose.")]
    [SerializeField] private string tooltip;
#endif

    /// <summary>
    /// The value of this variable
    /// </summary>
    public T Value 
    { 
        get 
        {
            return currentValue; 
        }
        set
        {
            OnVariableChanged?.Invoke(currentValue, ref value);

            currentValue = value;

            AfterVariableChanged?.Invoke(currentValue);
        } 
    }

    public delegate void VariableChanged(T oldVal, ref T newVal);
    /// <summary>
    /// Invoked when the value of the variable is changed. Can be used to modify the value the variable is being changed into.
    /// </summary>
    public event VariableChanged OnVariableChanged;

    /// <summary>
    /// Invoked after the variable is changed. Can be used to check the value of a variable after all modifications.
    /// </summary>
    public event System.Action<T> AfterVariableChanged;

    public void OnEnable()
    {
        if (Persistent) return;
        //Set currentValue to bypass all the code that runs from setting Value
        currentValue = initialValue;
        
        if (OnVariableChanged != null)
        {
            foreach (System.Delegate d in OnVariableChanged.GetInvocationList())
            {
                OnVariableChanged -= (VariableChanged)d;
            }
        }

        if (AfterVariableChanged != null)
        {
            foreach (System.Delegate d in AfterVariableChanged.GetInvocationList())
            {
                AfterVariableChanged -= (System.Action<T>)d;
            }
        }
    }

    public void OnValidate()
    {
        if (Application.isPlaying) UnityEditor.EditorApplication.delayCall += () => AfterVariableChanged?.Invoke(currentValue);
        else if (!Persistent) currentValue = initialValue;
    }

    public void ResetVariable(bool forced = false)
    {
        if (Persistent && !forced) return;

        currentValue = initialValue;
        AfterVariableChanged?.Invoke(Value);
    }
}
