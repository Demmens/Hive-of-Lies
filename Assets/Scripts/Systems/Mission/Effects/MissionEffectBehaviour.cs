using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class MissionEffectBehaviour : NetworkBehaviour
{
    public event System.Action OnEffectEnded;
    [SerializeField] bool shouldDestroyOnEnd = true;

    public override void OnStartServer()
    {
        //Effect is destroyed when the effect is over
        if (shouldDestroyOnEnd) OnEffectEnded += () => Destroy(this);

        OnEffectTriggered();
    }

    /// <summary>
    /// Call to end this effect
    /// </summary>
    public void EndEffect()
    {
        OnEffectEnded?.Invoke();
    }

    public abstract void OnEffectTriggered();
}
