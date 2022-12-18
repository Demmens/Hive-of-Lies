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
    [Tooltip("Dictionary of all players by their NetworkConnection")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("Set of all players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;
    #endregion

    public void AfterSetup()
    {
        allPlayers.Value.ForEach(ply => ply.OnFavourChanged += ChangeFavourUI);
    }

    [TargetRpc]
    public void ChangeFavourUI(NetworkConnection conn, int change)
    {
        favourText.text = change.ToString();
        favour.Value = change;
    }
}