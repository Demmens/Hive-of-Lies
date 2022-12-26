using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class ClientSelectPartners : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] PlayerButtonDropdown dropDown;
    [SerializeField] GameObject pickPlayerButton;
    [SerializeField] GameObject unpickPlayerButton;
    [SerializeField] GameObject lockInButton;

    List<ulong> pickedPlayers;
    bool canPick;
    #endregion

    #region SERVER
    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;

    [Tooltip("Invoked when the team leader locks their choices for partners in")]
    [SerializeField] GameEvent onTeamLeaderLockedIn;
    #endregion

    [Client]
    public override void OnStartClient()
    {
        PlayerButtonDropdownItem item = dropDown.CreateItem(pickPlayerButton);
        item.OnItemClicked += PlayerAdded;
        item = dropDown.CreateItem(unpickPlayerButton);
        item.OnItemClicked += PlayerRemoved;
    }

    [Server]
    public void TeamLeaderCanPick()
    {
        Debug.Log("Telling the team leader they can pick");
        CanStartPicking(teamLeader.Value.Connection);
    }

    [TargetRpc]
    void CanStartPicking(NetworkConnection conn)
    {
        Debug.Log("Can starty pcking now. :)");
        pickedPlayers = new List<ulong>();
        canPick = true;
        dropDown.AddAll(pickPlayerButton);
    }

    [Client]
    public void PlayerAdded(ulong playerID)
    {
        pickedPlayers.Add(playerID);
        dropDown.AddItem(playerID, unpickPlayerButton);
        dropDown.RemoveItem(playerID, pickPlayerButton);

        if (pickedPlayers.Count == ClientGameInfo.singleton.MaxPartners)
        {
            lockInButton.SetActive(true);
            dropDown.RemoveAll(pickPlayerButton);
        }
    }

    [Client]
    void PlayerRemoved(ulong playerID)
    {
        pickedPlayers.Remove(playerID);
        dropDown.AddItem(playerID, pickPlayerButton);
        dropDown.RemoveItem(playerID, unpickPlayerButton);

        if (pickedPlayers.Count == ClientGameInfo.singleton.MaxPartners - 1)
        {
            lockInButton.SetActive(false);
            dropDown.AddAll(pickPlayerButton);
            for (int i = 0; i < pickedPlayers.Count; i++ )
            {
                dropDown.RemoveItem(pickedPlayers[i], pickPlayerButton);
            }
        }
    }

    [Client]
    public void LockIn()
    {
        lockInButton.SetActive(false);
        canPick = false;
        dropDown.RemoveAll(pickPlayerButton);
        dropDown.RemoveAll(unpickPlayerButton);

        ServerLockIn();
    }

    [Command(requiresAuthority = false)]
    public void ServerLockIn(NetworkConnectionToClient conn = null)
    {
        if (conn != teamLeader.Value.Connection) return;

        onTeamLeaderLockedIn?.Invoke();
    }
}
