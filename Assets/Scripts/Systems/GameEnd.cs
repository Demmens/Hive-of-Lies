using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameEnd : NetworkBehaviour
{
    #region SERVER
    /// <summary>
    /// Amount of research needed for the Bees to win the game
    /// </summary>
    [SerializeField] IntVariable ResearchNeededForWin;

    /// <summary>
    /// How much honey has to be stolen before the wasps win
    /// </summary>
    [SerializeField] IntVariable HoneyNeededForWin;

    [SerializeField] IntVariable HoneyStolen;

    [SerializeField] IntVariable ResearchProgress;

    [SerializeField] HoLPlayerDictionary playersByConnection;
    #endregion
    #region CLIENT
    [SerializeField] GameObject gameEndScreen;
    #endregion

    public override void OnStartServer()
    {
        HoneyStolen.AfterVariableChanged += change =>
        {
            if (change >= HoneyNeededForWin) WaspsWin();
        };

        ResearchProgress.AfterVariableChanged += change =>
        {
            if (change >= ResearchNeededForWin) BeesWin();
        };
    }

    [Server]
    public void BeesWin()
    {
        GameObject screen = Instantiate(gameEndScreen);
        screen.GetComponent<PlayAgainButton>().SetText("Bees Win");
        NetworkServer.Spawn(screen);
    }

    [Server]
    public void WaspsWin()
    {
        GameObject screen = Instantiate(gameEndScreen);
        screen.GetComponent<PlayAgainButton>().SetText("Wasps Win");
        NetworkServer.Spawn(screen);
    }

    [Server]
    public void PlayerWins(NetworkConnection conn)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        GameObject screen = Instantiate(gameEndScreen);
        screen.GetComponent<PlayAgainButton>().SetText($"{ply.DisplayName} won the game");
        NetworkServer.Spawn(screen);
    }
}