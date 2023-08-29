using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FraudAbility : RoleAbility
{
    [SerializeField] NetworkingEvent playerWins;

    public void OnStung(NetworkConnection conn)
    {
        if (conn != Owner.connectionToClient) return;

        playerWins.Invoke(Owner.connectionToClient);
    }
}
