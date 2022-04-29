using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class ClientSelectPartners : MonoBehaviour
{
    public static ClientSelectPartners singleton;
    [SerializeField] PlayerButtonDropdown dropDown;
    [SerializeField] GameObject pickPlayerButton;
    [SerializeField] GameObject unpickPlayerButton;
    [SerializeField] GameObject lockInButton;

    List<ulong> pickedPlayers;
    bool canPick;

    void Start()
    {
        singleton = this;
        ClientEventProvider.singleton.OnTeamLeaderStartPicking += CanStartPicking;
        ClientEventProvider.singleton.OnPlayerClicked += PlayerClicked;
    }

    void CanStartPicking(ulong teamLeaderID)
    {
        if (ClientGameInfo.TeamLeaderID == SteamUser.GetSteamID())
        {
            pickedPlayers = new List<ulong>();
            canPick = true;
            dropDown.AddItem(pickPlayerButton);
        }
    }

    public void PlayerPicked(ulong playerID, bool added)
    {
        if (added)
        {
            pickedPlayers.Add(playerID);
            dropDown.AddItem(unpickPlayerButton);
            dropDown.RemoveItem(pickPlayerButton);

            if (pickedPlayers.Count == ClientGameInfo.MaxPartners)
            {
                lockInButton.SetActive(true);
            }
        }
        else
        {
            pickedPlayers.Remove(playerID);
            dropDown.AddItem(pickPlayerButton);
            dropDown.RemoveItem(unpickPlayerButton);
            lockInButton.SetActive(false);
        }

        NetworkClient.Send(new TeamLeaderChangePartnersMsg()
        {
            selected = added,
            playerID = new CSteamID(playerID)
        });
    }

    void PlayerClicked(ulong playerID)
    {
        if (!canPick) return;

        if (pickedPlayers.Contains(playerID))
        {
            dropDown.AddItem(unpickPlayerButton);
            dropDown.RemoveItem(pickPlayerButton);
        }
        else
        {
            if (pickedPlayers.Count < ClientGameInfo.MaxPartners) dropDown.AddItem(pickPlayerButton);
            dropDown.RemoveItem(unpickPlayerButton);
        }
    }

    public void LockIn()
    {
        lockInButton.SetActive(false);
        canPick = false;
        dropDown.RemoveItem(pickPlayerButton);
        dropDown.RemoveItem(unpickPlayerButton);

        NetworkClient.Send(new TeamLeaderLockInMsg() { });
    }
}
