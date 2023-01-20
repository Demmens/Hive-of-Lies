using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class ClientSelectPartners : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] PlayerButtonController buttonController;
    [SerializeField] GameObject pickPlayerButton;
    [SerializeField] GameObject unpickPlayerButton;
    [SerializeField] GameObject lockInButton;

    List<ulong> pickedPlayers;
    int clientNumPartners;
    #endregion

    #region SERVER
    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;

    [Tooltip("All players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;

    [Tooltip("The number of partners that are allowed on the mission")]
    [SerializeField] IntVariable numPartners;

    [Tooltip("Players that are currently selected")]
    [SerializeField] HoLPlayerSet playersSelected;

    [Tooltip("Invoked when the team leader locks their choices for partners in")]
    [SerializeField] GameEvent onTeamLeaderLockedIn;
    #endregion

    [Client]
    public override void OnStartClient()
    {
        PlayerButtonDropdownItem item = buttonController.CreateItem(pickPlayerButton);
        item.OnItemClicked += PlayerAdded;
        item = buttonController.CreateItem(unpickPlayerButton);
        item.OnItemClicked += PlayerRemoved;
    }

    [Server]
    public void TeamLeaderCanPick()
    {
        CanStartPicking(teamLeader.Value.Connection, numPartners);
    }

    [TargetRpc]
    void CanStartPicking(NetworkConnection conn, int partners)
    {
        pickedPlayers = new();
        buttonController.AddAll(pickPlayerButton);
        clientNumPartners = partners;
    }

    [Client]
    public void PlayerAdded(ulong playerID)
    {
        pickedPlayers.Add(playerID);
        buttonController.AddItem(playerID, unpickPlayerButton);
        buttonController.RemoveItem(playerID, pickPlayerButton);

        if (pickedPlayers.Count == clientNumPartners)
        {
            lockInButton.SetActive(true);
            buttonController.RemoveAll(pickPlayerButton);
        }

        ServerAddPlayer(playerID);
    }

    [Command(requiresAuthority = false)]
    void ServerAddPlayer(ulong player, NetworkConnectionToClient conn = null)
    {
        //Ignore if called by a player that isn't the team leader
        if (conn != teamLeader.Value.Connection) return;

        if (playersSelected.Value.Count >= numPartners) return;

        allPlayers.Value.ForEach(ply =>
        {
            if (ply.PlayerID == player && !playersSelected.Value.Contains(ply))
            {
                Debug.Log($"{teamLeader.Value.DisplayName} has selected {ply.DisplayName}");
                playersSelected.Add(ply);
            }
        });
    }

    [Client]
    void PlayerRemoved(ulong playerID)
    {
        pickedPlayers.Remove(playerID);
        buttonController.AddItem(playerID, pickPlayerButton);
        buttonController.RemoveItem(playerID, unpickPlayerButton);

        if (pickedPlayers.Count == clientNumPartners - 1)
        {
            lockInButton.SetActive(false);
            buttonController.AddAll(pickPlayerButton);
            for (int i = 0; i < pickedPlayers.Count; i++ )
            {
                buttonController.RemoveItem(pickedPlayers[i], pickPlayerButton);
            }
        }

        ServerRemovePlayer(playerID);
    }

    [Command(requiresAuthority = false)]
    void ServerRemovePlayer(ulong player, NetworkConnectionToClient conn = null)
    {
        //Ignore if called by a player that isn't the team leader
        if (conn != teamLeader.Value.Connection) return;

        allPlayers.Value.ForEach(ply =>
        {
            if (ply.PlayerID == player && playersSelected.Value.Contains(ply))
            {
                Debug.Log($"{teamLeader.Value.DisplayName} has deselected {ply.DisplayName}");
                playersSelected.Remove(ply);
            }
        });
    }

    [Client]
    public void LockIn()
    {
        lockInButton.SetActive(false);
        buttonController.RemoveAll(pickPlayerButton);
        buttonController.RemoveAll(unpickPlayerButton);

        ServerLockIn();
    }

    [Command(requiresAuthority = false)]
    public void ServerLockIn(NetworkConnectionToClient conn = null)
    {
        if (conn != teamLeader.Value.Connection) return;

        onTeamLeaderLockedIn?.Invoke();
    }
}
