using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Team", menuName = "Enums/Team")]
public class ETeam : ScriptableObject
{
    public Team Team;
    [Tooltip("The singular name of this team ('Wasp')")]
    public LocalizedString Name_sg;

    [Tooltip("The plural name of this team ('Wasps')")]
    public LocalizedString Name_pl;

    [Tooltip("The singular name of this team with indefinte article ('a Wasp')")]
    public LocalizedString Name_indef;

    [Tooltip("The singular name of this team with definite article ('the Wasp')")]
    public LocalizedString Name_def_sg;

    [Tooltip("The plural name of this team with definite article ('the Wasps')")]
    public LocalizedString Name_def_pl;
}