using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Buzz : NetworkBehaviour
{
    #region CLIENT

    [SerializeField] GameObject buzzOverlay;
    [SerializeField] GameObject buzzPlayerButtonPrefab;
    [SerializeField] Transform buzzPlayerButtonParent;
    [SerializeField] GameObject buzzVotes;
    [SerializeField] TMPro.TMP_Text playersBuzzedText;

    List<GameObject> buzzButtons = new();

    [SerializeField] GameObject submitButton;

    [SerializeField] IntVariable favour;
    #endregion
    #region SERVER
    [SerializeField] GameEvent BeesWin;
    [SerializeField] GameEvent WaspsWin;

    [SerializeField] hivePlayerDictionary playersByConnection;
    [SerializeField] hivePlayerSet alivePlayers;
    [SerializeField] hivePlayerSet waspPlayers;

    [SyncVar(hook = nameof(BuzzedPlayersChanged))] string playersBuzzed;

    /// <summary>
    /// Whether the buzz vote was unanimous or not
    /// </summary>
    bool unanimous;

    /// <summary>
    /// How many players have voted so far
    /// </summary>
    int numVotes;

    /// <summary>
    /// Total number of players voting in this buzz
    /// </summary>
    int maxVotes;

    /// <summary>
    /// How many bees are selected. If > 0 the buzz will not be successful
    /// </summary>
    int beesInBuzz;

    /// <summary>
    /// Whether the buzz is a success (all selected players are wasps)
    /// </summary>
    bool successful => beesInBuzz == 0;

    [SerializeField] [SyncVar] int buzzCost = 5;

    /// <summary>
    /// The player who buzzed in the current buzz
    /// </summary>
    hivePlayer currentBuzzer = null;

    List<hivePlayer> hasBuzzed = new();
    List<hivePlayer> selectedPlayers = new();
    #endregion


    /// <summary>
    /// Called when a player clicks the buzz button
    /// </summary>
    [Client]
    public void BuzzClicked()
    {
        if (favour < buzzCost) return;
        ServerOnBuzz();
    }
    
    /// <summary>
    /// Called on the server when a player clicks the buzz button
    /// </summary>
    [Command(requiresAuthority = false)]
    void ServerOnBuzz(NetworkConnectionToClient conn = null)
    {
        //If someone is currently buzzing
        if (currentBuzzer != null) return;
        //If the player is dead / not in the game
        if (!playersByConnection.Value.TryGetValue(conn, out hivePlayer ply)) return;
        //If they've buzzed before
        if (hasBuzzed.Contains(ply)) return;
        //If they cannot afford it
        if (ply.Favour < buzzCost) return;
        hasBuzzed.Add(ply);
        //Set this player as the current buzzer
        currentBuzzer = ply;
        ply.Favour.Value -= buzzCost;
        //Set unanimous to true. When a player votes no, we can set this to false.
        unanimous = true;
        numVotes = 0;
        beesInBuzz = 0;
        maxVotes = 0;
        selectedPlayers = new();

        List<ulong> plyIds = new();
        List<string> plyNames = new();

        alivePlayers.Value.ForEach(pl =>
        {
            //Don't create a button for the player buzzing
            if (pl != ply)
            {
                plyIds.Add(pl.PlayerID);
                plyNames.Add(pl.DisplayName);
            }
        });
        ActivateBuzz();
        ActivateBuzzButtons(ply.connectionToClient, plyIds, plyNames);
    }

    /// <summary>
    /// Creates the buzz overlay on all clients
    /// </summary>
    [ClientRpc]
    void ActivateBuzz()
    {
        buzzOverlay.SetActive(true);
    }

    [TargetRpc]
    void ActivateBuzzButtons(NetworkConnection conn, List<ulong> plyIds, List<string> plyNames)
    {
        for (int i = 0; i < plyIds.Count; i++)
        {
            GameObject button = Instantiate(buzzPlayerButtonPrefab);
            button.transform.SetParent(buzzPlayerButtonParent);

            BuzzButton btn = button.GetComponent<BuzzButton>();
            btn.PlayerName.text = plyNames[i];
            btn.ID = plyIds[i];
            btn.OnClicked += id => PlayerClicked(id);

            buzzButtons.Add(button);
        }
    }

    /// <summary>
    /// Called when the buzzing player clicks a player to add to or remove from the buzz
    /// </summary>
    [Command(requiresAuthority = false)]
    public void PlayerClicked(ulong id, NetworkConnectionToClient conn = null)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out hivePlayer ply)) return;
        if (ply != currentBuzzer) return;

        hivePlayer target = null;

        alivePlayers.Value.ForEach(pl =>
        {
            if (pl.PlayerID == id) target = pl;
        });

        if (target == null) return;

        if (selectedPlayers.Contains(target)) RemoveFromBuzz(target);
        else AddToBuzz(target);
    }

    [Server]
    void AddToBuzz(hivePlayer ply)
    {
        selectedPlayers.Add(ply);
        WriteBuzzedString();

        if (ply.Team.Value.Team == Team.Bee) beesInBuzz++;

        if (selectedPlayers.Count == waspPlayers.Value.Count) SetSubmitActive(currentBuzzer.connectionToClient, true);
    }

    [Server]
    void RemoveFromBuzz(hivePlayer ply)
    {
        selectedPlayers.Remove(ply);
        WriteBuzzedString();

        if (ply.Team.Value.Team == Team.Bee) beesInBuzz--;

        if (selectedPlayers.Count == waspPlayers.Value.Count - 1) SetSubmitActive(currentBuzzer.connectionToClient, false);
    }

    [TargetRpc]
    void SetSubmitActive(NetworkConnection conn, bool active)
    {
        submitButton.SetActive(active);
    }

    [Server]
    void WriteBuzzedString()
    {
        string buzzed = "";
        selectedPlayers.ForEach(pl => buzzed += pl.DisplayName + "\n");
        playersBuzzed = buzzed.TrimEnd('\n');
    }

    [ClientRpc]
    void BuzzedPlayersChanged(string oldVal, string newVal)
    {
        playersBuzzedText.text = newVal;
    }

    [Client]
    public void BuzzSubmitted()
    {
        ServerBuzzSubmitted();
        submitButton.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    void ServerBuzzSubmitted(NetworkConnectionToClient conn = null)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out hivePlayer ply)) return;
        if (ply != currentBuzzer) return;

        for (int i = 0; i < alivePlayers.Value.Count; i++)
        {
            hivePlayer pl = alivePlayers.Value[i];

            if (!CanVote(pl)) return;
            maxVotes++;
            //All other players get to vote
            ShowVoteButtons(pl.connectionToClient);
        }
    }

    /// <summary>
    /// Returns true if a player is allowed to vote on the buzz
    /// </summary>
    /// <param name="ply"></param>
    /// <returns></returns>
    [Server]
    bool CanVote(hivePlayer ply)
    {
        //Current buzzer does not get to vote (assumed that they vote yes) to prevent time-wasting buzzes
        if (ply == currentBuzzer) return false;

        //Players who have been selected do not get to vote (they have no reason to vote yes)
        if (selectedPlayers.Contains(ply)) return false;

        return true;
    }

    [TargetRpc]
    void ShowVoteButtons(NetworkConnection conn)
    {
        buzzVotes.SetActive(true);
    }

    [Client]
    public void ClientVoteClicked(bool yesVote)
    {
        OnPlayerVoted(yesVote);
        buzzVotes.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    void OnPlayerVoted(bool yesVote, NetworkConnectionToClient conn = null)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out hivePlayer ply)) return;
        //If the player shouldn't be able to vote, don't let them.
        if (!CanVote(ply)) return;

        //If they voted no, the vote is never going to be unanimous
        if (!yesVote) unanimous = false;

        if (++numVotes == maxVotes) OnFinalVote();
    }

    /// <summary>
    /// Called when the final player places their vote
    /// </summary>
    [Server]
    void OnFinalVote()
    {
        if (unanimous)
        {
            if (successful) BeesWin?.Invoke();
            else WaspsWin?.Invoke();
        }

        currentBuzzer = null;
        buzzOverlay.SetActive(false);
        buzzButtons.ForEach(btn => Destroy(btn));
    }
}
