using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Mirror;

[CreateAssetMenu(fileName = "Game Mode", menuName = "Game Mode")]
public class GameMode : ScriptableObject
{
    [field:SerializeField]
    public LocalizedString Name { get; private set; }

    [field:SerializeField]
    public LocalizedString Description { get; private set; }

    [field:SerializeField]
    public int MinPlayers;

    [field: SerializeField]
    public int MaxPlayers;

    [Scene]
    [field:SerializeField]
    public string GameScene;

    public List<MissionList> MissionLists;
}