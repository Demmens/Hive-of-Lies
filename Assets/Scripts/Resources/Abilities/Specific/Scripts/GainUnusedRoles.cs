using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GainUnusedRoles : RoleAbility
{
    [SerializeField] int rolesToGain = 2;

    #region SERVER
    [SerializeField] RoleDataSet rejectedRoles;
    [SerializeField] HoLPlayerSet waspPlayers;
    [SerializeField] RoleAbility noAbility;

    [HideInInspector]
    public List<Role> Roles;

    string roleString;
    #endregion
    #region CLIENT
    [SerializeField] GameObject popup;
    #endregion    

    [Server]
    public void AfterAllRolesChosen()
    {
        rejectedRoles.Value.Shuffle();
        int pickedRoles = 0;

        for (int i = 0; pickedRoles < rolesToGain && i < rejectedRoles.Value.Count; i++)
        {
            RoleData rl = rejectedRoles.Value[i];
            //Make sure they're not given a role that has no ability
            if (rl.Abilities.Count == 0) continue;
            if (rl.Team == Team.Wasp) continue;
            GiveRole(rl);
            pickedRoles++;
            roleString += rl.RoleName + "\n";
        }

        roleString = roleString.TrimEnd('\n');
        CreatePopup(roleString);

        foreach (HoLPlayer ply in waspPlayers.Value)
        {
            if (ply.Target == Owner) CreateTargetPopup(ply.connectionToClient, roleString);
            //If the player already has a target, then we know we can skip the event subscribing
            if (ply.Target != null) return;
            ply.Target.AfterVariableChanged += (target) =>
            {
                if (target == Owner) CreateTargetPopup(ply.connectionToClient, roleString);
            };
        }
    }

    [TargetRpc]
    void CreatePopup(string roles)
    {
        popup = Instantiate(popup);
        popup.GetComponent<Notification>().SetText("Your roles are:\n" + roles);
    }

    [TargetRpc]
    void CreateTargetPopup(NetworkConnection conn, string str)
    {
        popup = Instantiate(popup);
        popup.GetComponent<Notification>().SetText("The Two Bees in a Trench Coat has the roles:\n" + str);
    }

    [Server]
    void GiveRole(RoleData data)
    {
        List<RoleAbility> abilities = new();
        for (int i = 0; i < data.Abilities.Count; i++)
        {
            GameObject abilityObject = Instantiate(data.Abilities[i]);
            NetworkServer.Spawn(abilityObject, Owner.connectionToClient);

            RoleAbility[] scripts = abilityObject.GetComponents<RoleAbility>();

            foreach (RoleAbility ability in scripts)
            {
                ability.Owner = Owner;
                abilities.Add(ability);
            }
        }

        Role role = new()
        {
            Abilities = abilities,
            Data = data
        };

        Roles.Add(role);
    }
}
