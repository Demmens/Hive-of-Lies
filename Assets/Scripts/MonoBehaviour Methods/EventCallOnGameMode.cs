using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class EventCallOnGameMode : NetworkBehaviour
{
    [SerializeField] UnityEvent OnGameModeBeginServer;
    [SerializeField] UnityEvent OnGameModeBeginClient;
    [SerializeField] GameModeVariable currentMode;
    [SerializeField] List<GameMode> targetModes;
    [SerializeField] bool sendToClients = true;
    [SyncVar] bool isGameMode; 

    public override void OnStartServer()
    {
        if (!targetModes.Contains(currentMode.Value)) return;
        isGameMode = true;

        //Delay by one frame so other Start methods have a chance to trigger (and listen to variable changes or game events)
        StartCoroutine(Coroutines.Delay(OnGameModeBeginServer.Invoke));
    }

    public override void OnStartClient()
    {
        if (!isGameMode) return;

        //Delay by one frame so other Start methods have a chance to trigger (and listen to variable changes or game events)
        StartCoroutine(Coroutines.Delay(OnGameModeBeginClient.Invoke));
    }
}
