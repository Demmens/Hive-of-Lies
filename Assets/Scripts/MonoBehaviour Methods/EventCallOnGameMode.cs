using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCallOnGameMode : MonoBehaviour
{
    [SerializeField] UnityEvent OnGameModeBegin;
    [SerializeField] GameModeVariable currentMode;
    [SerializeField] List<GameMode> targetModes;

    private void Start()
    {
        Debug.Log(currentMode.Value);
        if (targetModes.Contains(currentMode.Value)) OnGameModeBegin.Invoke();
    }
}
