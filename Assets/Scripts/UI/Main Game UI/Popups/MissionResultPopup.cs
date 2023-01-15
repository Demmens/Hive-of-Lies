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
    [SerializeField] TMP_Text rollResults;

    [SerializeField] GameObject effectPrefab;
    [SerializeField] Transform effectParent;

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

    [Tooltip("All players that have closed the popup so far")]
    List<NetworkConnection> playersClosed;

    [Tooltip("The playercount of the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("The total value of played cards")]
    [SerializeField] IntVariable cardsTotal;

    [Tooltip("The current mission")]
    [SerializeField] MissionVariable currentMission;

    [Tooltip("Invoked when all players have closed the popup")]
    [SerializeField] GameEvent allPlayersClosed;

    List<GameObject> effectTiers = new();
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

            CreatePopup(ply.Connection, "", played, currentMission, cardsTotal);
        });
    }

    [TargetRpc]
    public void CreatePopup(NetworkConnection conn, string flavour, List<Card> contributions, Mission currentMission, int cardsTotal) 
    {
        foreach (GameObject obj in effectTiers) Destroy(obj);
        effectTiers = new ();
        string cardResultsText = "";

        for (int i = 0; i < contributions.Count; i++)
        {
            if (i != 0) cardResultsText += ", ";
            cardResultsText += contributions[i].Value.ToString();
        }

        if (!isOnMission) cardResultsText = "??";

        rollResults.text = cardResultsText;

        foreach (MissionEffectTier tier in currentMission.effects) {
            if (!tier.Applicable(cardsTotal)) continue;

            GameObject effect = Instantiate(effectPrefab);
            effectTiers.Add(effect);
            effect.transform.SetParent(effectParent);

            effect.GetComponent<MissionEffectText>().SetText(tier.comparator, tier.value, tier.effects);
        }

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
