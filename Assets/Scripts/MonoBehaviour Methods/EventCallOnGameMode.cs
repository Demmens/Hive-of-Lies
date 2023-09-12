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
        if (!targetModes.Contains(currentMode.Value)) return;

        //Delay by one frame so other Start methods have a chance to trigger (and listen to variable changes or game events)
        StartCoroutine(Coroutines.Delay(OnGameModeBegin.Invoke));
    }
}
