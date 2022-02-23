using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class PlayerButton : MonoBehaviour
{
    /// <summary>
    /// Private counterpart to <see cref="SteamID"/>
    /// </summary>
    private CSteamID steamID;
    /// <summary>
    /// Steam ID of the player associated with this button
    /// </summary>
    public CSteamID SteamID
    {
        get
        {
            return steamID;
        }
        set
        {
            steamID = value;
            gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = SteamFriends.GetFriendPersonaName(value);
        }
    }

    /// <summary>
    /// Whether this is selected or not
    /// </summary>
    public bool selected;

    public void OnClicked()
    {
        Debug.Log("clicked");
        if (SteamUser.GetSteamID() == ClientGameInfo.TeamLeaderID && ClientGameInfo.CurrentlySelected.Count < ClientGameInfo.MaxPartners)
        {
            Debug.Log("Click attempt");
            selected = true;
            ClientGameInfo.CurrentlySelected.Add(steamID);
            NetworkClient.Send(new TeamLeaderChangePartnersMsg()
            {
                selected = true,
                playerID = steamID
            });
            if (ClientGameInfo.CurrentlySelected.Count == ClientGameInfo.MaxPartners)
            {
                NetworkClient.Send(new TeamLeaderLockInMsg()
                {

                });
            }
        }
    }
}
