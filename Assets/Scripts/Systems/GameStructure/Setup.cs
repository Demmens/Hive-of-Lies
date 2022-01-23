using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{

    [SerializeField] Player PlayerPrefab;

    /// <summary>
    /// Number of role choices given to innocents
    /// </summary>
    [SerializeField] int InnocentRoleChoices = 3;

    /// <summary>
    /// Number of role choices given to traitors
    /// </summary>
    [SerializeField] int TraitorRoleChoices = 3;

    /// <summary>
    /// Percentage of players that will be a traitor
    /// </summary>
    [SerializeField] float TraitorRatio = 0.49f;

}
