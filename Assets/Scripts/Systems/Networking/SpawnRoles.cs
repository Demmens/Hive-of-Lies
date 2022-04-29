using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnRoles : NetworkBehaviour
{
    public static SpawnRoles singleton;
    // Start is called before the first frame update
    void Start()
    {
        singleton = singleton ?? this;
    }

    [TargetRpc]
    public void SpawnRoleAbilityOnClient(NetworkConnection target)
    {

    }
}
