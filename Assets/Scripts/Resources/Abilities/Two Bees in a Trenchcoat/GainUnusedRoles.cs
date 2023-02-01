using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GainUnusedRoles : RoleAbility
{
    [SerializeField] int rolesToGain = 2;
    [SerializeField] GameObject popup;

    [SerializeField] RoleDataSet rejectedRoles;
    [SerializeField] RoleAbility noAbility;

    [HideInInspector]
    public List<Role> Roles;

    [Server]
    public void AfterAllRolesChosen()
    {
        rejectedRoles.Value.Shuffle();
        string roleString = "";
        int favour = 0;
        int pickedRoles = 0;

        for (int i = 0; pickedRoles < rolesToGain && i < rejectedRoles.Value.Count; i++)
        {
            RoleData rl = rejectedRoles.Value[i];
            //Make sure they're not given a role that has no ability
            if (rl.Ability == noAbility) continue;
            GiveRole(rl);
            pickedRoles++;
            roleString += rl.RoleName + "\n";
            favour += rl.StartingFavour;
        }
        favour /= rolesToGain;
        favour /= 2;
        Owner.Favour.Value += favour;

        roleString = roleString.TrimEnd('\n');
        CreatePopup(roleString);
    }

    [TargetRpc]
    void CreatePopup(string roles)
    {
        popup = Instantiate(popup);
        popup.GetComponent<Notification>().SetText("Your roles are:\n" + roles);
    }

    [Server]
    void GiveRole(RoleData data)
    {
        GameObject abilityObject = Instantiate(data.Ability);
        NetworkServer.Spawn(abilityObject, Owner.connectionToClient);

        RoleAbility[] abilities = abilityObject.GetComponents<RoleAbility>();

        foreach (RoleAbility ability in abilities)
        {
            ability.Owner = Owner;
            ability.OnRoleGiven();
        }

        Role role = new()
        {
            Abilities = abilities,
            Data = data
        };

        Roles.Add(role);
    }
}
