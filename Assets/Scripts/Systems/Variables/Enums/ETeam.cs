using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Team", menuName = "Enums/Team")]
public class ETeam : ScriptableObject
{
    public Team Team;

    [Tooltip("The singular name of this team ('Wasp')")]
    [SerializeField] LocalizedString name_sg;
    
    /// <summary>
    /// The singular name of this team ('Wasp')
    /// </summary>
    public string Name_sg
    {
        get
        {
            return name_sg.GetLocalizedString();
        }
    }

    [Tooltip("The plural name of this team ('Wasps')")]
    [SerializeField] LocalizedString name_pl;

    /// <summary>
    /// The plural name of this team ('Wasps')
    /// </summary>
    public string Name_pl
    {
        get
        {
            return name_pl.GetLocalizedString();
        }
    }

    [Tooltip("The singular name of this team with indefinte article ('a Wasp')")]
    [SerializeField] LocalizedString name_indef;
    /// <summary>
    /// The singular name of this team with indefinte article ('a Wasp')
    /// </summary>
    public string Name_indef
    {
        get
        {
            return name_indef.GetLocalizedString();
        }
    }

    [Tooltip("The singular name of this team with definite article ('the Wasp')")]
    [SerializeField] LocalizedString name_def_sg;

    /// <summary>
    /// The singular name of this team with definite article ('the Wasp')
    /// </summary>
    public string Name_def_sg
    {
        get
        {
            return name_def_sg.GetLocalizedString();
        }
    }

    [Tooltip("The plural name of this team with definite article ('the Wasps')")]
    [SerializeField] LocalizedString name_def_pl;

    /// <summary>
    /// The plural name of this team with definite article ('the Wasps')
    /// </summary>
    public string Name_def_pl
    {
        get
        {
            return name_def_pl.GetLocalizedString();
        }
    }
}