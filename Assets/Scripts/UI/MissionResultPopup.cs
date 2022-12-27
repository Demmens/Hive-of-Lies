using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class MissionResultPopup : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] GameObject popup;
    [SerializeField] TMP_Text missionResultText;
    [SerializeField] TMP_Text outcomeFlavour;
    [SerializeField] TMP_Text rollResults;

    [Tooltip("Whether the local player is on the mission")]
    [SerializeField] BoolVariable isOnMission;
    #endregion

    #region SERVER
    [Tooltip("Set of all players")]
    [SerializeField] HoLPlayerSet allPlayers;

    [Tooltip("Set of all players on the mission")]
    [SerializeField] HoLPlayerSet playersOnMission;

    [Tooltip("Set of all played cards this mission")]
    [SerializeField] CardSet playedCards;

    [Tooltip("The result of the last completed mission")]
    [SerializeField] MissionResultVariable missionResult;

    [Tooltip("All players that have closed the popup so far")]
    List<NetworkConnection> playersClosed;

    [Tooltip("The playercount of the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("Invoked when all players have closed the popup")]
    [SerializeField] GameEvent allPlayersClosed;
    #endregion

    [Server]
    public void OnMissionEnd()
    {
        playersClosed = new();
        allPlayers.Value.ForEach(ply =>
        {
            List<Card> played = new();

            //Show players on the mission what everyone played. Nobody else gets to see.
            if (playersOnMission.Value.Contains(ply)) played = playedCards;
            
            CreatePopup(ply.Connection, missionResult, played);
        });
    }

    [TargetRpc]
    public void CreatePopup(NetworkConnection conn, MissionResult result, List<Card> contributions)
    {
        string cardResultsText = "";

        for (int i = 0; i < contributions.Count; i++)
        {
            if (i != 0) cardResultsText += ", ";
            cardResultsText += isOnMission ? contributions[i].Value.ToString() : "??";
        }
        rollResults.text = cardResultsText;

        bool success = result == MissionResult.Success;

        missionResultText.text = success ? "Mission Succeeded" : "Mission Failed";

        outcomeFlavour.text = success ? "[Success Flavour]" : "[Fail Flavour]";

        popup.SetActive(true);
    }

    [Client]
    public void OnClose()
    {
        popup.SetActive(false);
        OnPlayerClosedPopup();
    }

    [Command(requiresAuthority = false)]
    void OnPlayerClosedPopup(NetworkConnectionToClient conn = null)
    {
        if (playersClosed.Contains(conn)) return;

        playersClosed.Add(conn);

        if (playersClosed.Count >= playerCount) allPlayersClosed?.Invoke();
    }
}
