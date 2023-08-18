using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Services.Analytics;

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

    [SerializeField] HoLPlayerSet waspPlayers;

    [SerializeField] GameObject gameEndScreen;

    [SerializeField] IntVariable roundNum;

    bool hasWon;
    #endregion

    public override void OnStartServer()
    {
        HoneyStolen.AfterVariableChanged += change =>
        {
            if (change >= HoneyNeededForWin) StartCoroutine(Coroutines.Delay(WaspsWin));
        };

        ResearchProgress.AfterVariableChanged += change =>
        {
            if (change >= ResearchNeededForWin) StartCoroutine(Coroutines.Delay(BeesWin));
        };

        waspPlayers.AfterItemRemoved += (item) =>
        {
            if (waspPlayers.Value.Count == 0) BeesWin();
        };
    }

    [Server]
    public void BeesWin()
    {
        if (hasWon) return;
        hasWon = true;
        //If 3 honey is stolen at the same time as 3 wasp facts are learned, the bees don't win
        if (HoneyStolen >= HoneyNeededForWin) return;
        GameObject screen = Instantiate(gameEndScreen);
        screen.GetComponent<PlayAgainButton>().SetText("BEES WIN");
        NetworkServer.Spawn(screen);

        Dictionary<string, object> parameters = new()
        {
            { "beePoints", ResearchProgress.Value},
            { "waspPoints", HoneyStolen.Value },
            { "waspsAlive", waspPlayers.Value.Count },
            { "team", "Bee"},
            { "playerCount", playersByConnection.Value.Count },
            { "roundNum",  roundNum.Value},
        };
        AnalyticsService.Instance.CustomData("hiveGameEnded", parameters);
    }

    [Server]
    public void WaspsWin()
    {
        if (hasWon) return;
        hasWon = true;
        GameObject screen = Instantiate(gameEndScreen);
        screen.GetComponent<PlayAgainButton>().SetText("WASPS WIN");
        NetworkServer.Spawn(screen);

        Dictionary<string, object> parameters = new()
        {
            { "beePoints", ResearchProgress.Value},
            { "waspPoints", HoneyStolen.Value },
            { "waspsAlive", waspPlayers.Value.Count },
            { "team", "Wasp"},
            { "playerCount", playersByConnection.Value.Count },
            { "roundNum",  roundNum.Value},
        };
        AnalyticsService.Instance.CustomData("hiveGameEnded", parameters);
    }

    [Server]
    public void PlayerWins(NetworkConnection conn)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        GameObject screen = Instantiate(gameEndScreen);
        screen.GetComponent<PlayAgainButton>().SetText($"{ply.DisplayName.ToUpper()} WINS");
        NetworkServer.Spawn(screen);

        Dictionary<string, object> parameters = new()
        {
            { "beePoints", ResearchProgress.Value},
            { "waspPoints", HoneyStolen.Value },
            { "waspsAlive", waspPlayers.Value.Count },
            { "team", "Solo"},
            { "playerCount", playersByConnection.Value.Count },
            { "roundNum",  roundNum.Value},
        };
        AnalyticsService.Instance.CustomData("hiveGameEnded", parameters);
    }
}