using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FavourController : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] TMPro.TMP_Text FavourText;
    #endregion

    #region SERVER
    [SerializeField] HoLPlayerDictionary playersByConnection;
    [SerializeField] HoLPlayerSet allPlayers;
    #endregion

    public void AfterSetup()
    {
        allPlayers.Value.ForEach(ply => ply.Favour.AfterVariableChanged += ChangeFavour);
    }

    public void ChangeFavour(int change)
    {
        
    }

    [Command(requiresAuthority = false)]
    void ServerChangeFavour()
    {

    }
}