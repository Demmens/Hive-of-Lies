using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychicAbility : RoleAbility
{
    [SerializeField] NetworkingEvent showTopCard;
    protected override void OnRoleGiven()
    {
        Owner.NumDraws.AfterVariableChanged += (int val) => showTopCard.Invoke(Owner.connectionToClient);
    }
}
