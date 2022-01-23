using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Framework for sending and receiving game events
/// </summary>
public class EventSystem : MonoBehaviour
{
    /// <summary>
    /// A dictonary containing events and objects that are bound to those events
    /// </summary>
    Dictionary<GameEvents, List<IEventListener>> eventListeners;

    /// <summary>
    /// Binds an object to an event. When this event is triggered, it will call the inherited ReceiveEvent function on the object
    /// <para></para><see cref="IEventListener.ReceiveEvent(GameEvents, IEventListener, EventParams)"/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="listener"></param>
    public void BindEvent(GameEvents key, IEventListener listener)
    {
        if (!eventListeners.TryGetValue(key, out var list))
        {
            list = new List<IEventListener>();
            eventListeners.Add(key, list);
        }
        list.Add(listener);
    }

    /// <summary>
    /// Broadcasts the specified event. This will trigger the ReceiveEvent function on any listeners
    /// <para></para><see cref="IEventListener.ReceiveEvent(GameEvents, IEventListener, EventParams)"/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="caller"></param>
    /// <param name="param"></param>
    public void BroadcastEvent(GameEvents key, IEventListener caller, EventParams param)
    {
        if (eventListeners.TryGetValue(key, out var list))
        {
            list.ForEach(listener =>
            {
                listener.ReceiveEvent(key, caller, param);
            });
        }
    }
}

/// <summary>
/// Parameters fed into the Broadcast and Receive event functions
/// </summary>
public struct EventParams
{

}

/// <summary>
/// An object that inherits this interface will be able to listen for game events
/// </summary>
public interface IEventListener
{
    void ReceiveEvent(GameEvents key, IEventListener caller, EventParams param);
}

/// <summary>
/// Contains all game events
/// </summary>
public enum GameEvents
{
    BeforeInfluenceResult
}