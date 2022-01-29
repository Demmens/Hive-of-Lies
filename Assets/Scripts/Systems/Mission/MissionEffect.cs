using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEffect : MonoBehaviour
{
    #region Properties
    /// <summary>
    /// Global events
    /// </summary>
    protected EventSystem events;

    /// <summary>
    /// Private counterpart to <see cref="Description"/>
    /// </summary>
    [SerializeField] string description;

    /// <summary>
    /// Description of the effect
    /// </summary>
    public string Description
    {
        get
        {
            return description;
        }
    }
    #endregion

    void Start()
    {
        events = FindObjectOfType<EventSystem>();
        BindEvents();
    }

    /// <summary>
    /// Trigger this mission effect
    /// </summary>
    public virtual void TriggerEffect() { }

    /// <summary>
    /// Bind any events we want to use for the mission effect
    /// </summary>
    public virtual void BindEvents() { }
}
