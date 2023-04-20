using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class ClientSelectPartners : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] GameObject pickPlayerButton;
    [SerializeField] GameObject unpickPlayerButton;
    [SerializeField] GameObject lockInButton;
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

    private int numPlayersForLockIn = 1;
    #endregion

    List<PlayerButtonDropdownItem> addItems = new();
    List<PlayerButtonDropdownItem> removeItems = new();

    [Server]
    public void TeamLeaderCanPick()
    {
        foreach (PlayerButtonDropdownItem i in addItems) Destroy(i);
        foreach (PlayerButtonDropdownItem i in removeItems) Destroy(i);

        foreach (HoLPlayer ply in allPlayers.Value)
        {
            CreateAddItem(ply);
        }
    }

    [Server]
    void AddPlayer(HoLPlayer ply, PlayerButtonDropdownItem item)
    {
        if (playersSelected.Value.Count >= numPartners) return;
        if (playersSelected.Value.Contains(ply)) return;

        Destroy(item);
        addItems.Remove(item);
        CreateRemoveItem(ply);

        Debug.Log($"{teamLeader.Value.DisplayName} has selected {ply.DisplayName}");
        playersSelected.Add(ply);

        if (playersSelected.Value.Count >= numPlayersForLockIn) SetLockInActive(teamLeader.Value.connectionToClient, true);

        if (playersSelected.Value.Count < numPartners) return;

        OnMaxPlayersAdded();
    }

    [Server]
    void OnMaxPlayersAdded()
    {
        foreach (PlayerButtonDropdownItem i in addItems)
        {
            Destroy(i);
        }
    }

    [Server]
    void RemovePlayer(HoLPlayer ply, PlayerButtonDropdownItem item)
    {
        if (!playersSelected.Value.Contains(ply)) return;

        Destroy(item);
        CreateAddItem(ply);

        Debug.Log($"{teamLeader.Value.DisplayName} has deselected {ply.DisplayName}");
        playersSelected.Remove(ply);

        if (playersSelected.Value.Count < numPlayersForLockIn) SetLockInActive(teamLeader.Value.connectionToClient, false);

        if (playersSelected.Value.Count < numPartners - 1) return;

        OnNoLongerMaxPlayersAdded(ply);
    }

    [Server]
    void OnNoLongerMaxPlayersAdded(HoLPlayer ply)
    {
        foreach (HoLPlayer pl in allPlayers.Value)
        {
            if (playersSelected.Value.Contains(pl)) continue;
            //If it's the person we've just removed, then the add to mission button is created elsewhere
            if (pl == ply) continue;

            CreateAddItem(pl);
        }
    }

    [TargetRpc]
    void SetLockInActive(NetworkConnection conn, bool active)
    {
        lockInButton.SetActive(active);
    }

    [Client]
    public void LockIn()
    {
        lockInButton.SetActive(false);
        ServerLockIn();
    }

    [Command(requiresAuthority = false)]
    public void ServerLockIn(NetworkConnectionToClient conn = null)
    {
        if (conn != teamLeader.Value.connectionToClient) return;

        foreach (PlayerButtonDropdownItem i in addItems) Destroy(i);
        foreach (PlayerButtonDropdownItem i in removeItems) Destroy(i);

        onTeamLeaderLockedIn?.Invoke();
    }

    void CreateAddItem(HoLPlayer ply)
    {
        PlayerButtonDropdownItem item = ply.Button.AddDropdownItem(pickPlayerButton, teamLeader);
        item.OnItemClicked += (ply) => AddPlayer(ply, item);
        addItems.Add(item);
    }

    void CreateRemoveItem(HoLPlayer ply)
    {
        PlayerButtonDropdownItem item = ply.Button.AddDropdownItem(unpickPlayerButton, teamLeader);
        item.OnItemClicked += (ply) => RemovePlayer(ply, item);
        removeItems.Add(item);
    }
}
