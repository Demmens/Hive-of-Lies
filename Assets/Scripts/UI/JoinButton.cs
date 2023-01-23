using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class JoinButton : MonoBehaviour
{
    public void OnClick()
    {
        SteamFriends.ActivateGameOverlay("friends");
    }
}
