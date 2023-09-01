using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FavourController : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] TMPro.TMP_Text favourText;

    [Tooltip("The favour of the local player")]
    [SerializeField] IntVariable favour;
    #endregion

    #region SERVER
    [Tooltip("Set of all players in the game")]
    [SerializeField] HivePlayerSet allPlayers;
    #endregion

    public void AfterSetup()
    {
        foreach (HivePlayer ply in allPlayers.Value)
        {
            ply.Favour.AfterVariableChanged += (val) => ChangeFavourUI(ply.connectionToClient, val);
        }
    }

    [TargetRpc]
    public void ChangeFavourUI(NetworkConnection conn, int change)
    {
        favourText.text = change.ToString();
        favour.Value = change;
    }
}