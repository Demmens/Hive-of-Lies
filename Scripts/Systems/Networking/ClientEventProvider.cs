using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientEventProvider : MonoBehaviour
{
    /// <summary>
    /// The sole client event provider
    /// </summary>
    public static ClientEventProvider singleton;

    public int test = 5;


    /// <summary>
    /// Fired when the team leader changes
    /// </summary>
    public event TeamLeaderChanged OnTeamLeaderChanged;
    public delegate void TeamLeaderChanged(TeamLeaderChangedMsg msg);

    void Start()
    {
        singleton = this;
        DontDestroyOnLoad(this);

        //Register events
        NetworkClient.RegisterHandler((TeamLeaderChangedMsg msg) => { OnTeamLeaderChanged?.Invoke(msg); });
    }
}
