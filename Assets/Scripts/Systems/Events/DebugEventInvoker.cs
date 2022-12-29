using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEventInvoker : MonoBehaviour
{
    [SerializeField] GameEvent eventToInvoke;

    [ContextMenu("Invoke Event")]
    private void InvokeEvent()
    {
        eventToInvoke?.Invoke();
    }
}
