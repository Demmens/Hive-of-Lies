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
    public int ResearchNeededForWin = 3;

    /// <summary>
    /// How much honey has to be stolen before the wasps win
    /// </summary>
    public int HoneyNeededForWin = 3;

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

    [ClientRpc]
    public void BeesWin()
    {
        GameObject screen = Instantiate(gameEndScreen);
        screen.GetComponent<PlayAgainButton>().SetText("Bees Win");
    }

    [ClientRpc]
    public void WaspsWin()
    {
        GameObject screen = Instantiate(gameEndScreen);
        screen.GetComponent<PlayAgainButton>().SetText("Wasps Win");
    }

    public void PlayerWins(NetworkConnection conn)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        ClientPlayerWins(ply.DisplayName);
    }

    [ClientRpc]
    public void ClientPlayerWins(string playerName)
    {
        GameObject screen = Instantiate(gameEndScreen);
        screen.GetComponent<PlayAgainButton>().SetText($"{playerName} won the game");
    }
}