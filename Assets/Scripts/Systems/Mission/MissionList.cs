using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionList", menuName = "Missions/Create mission list")]
public class MissionList : ScriptableObject
{
    [SerializeField] int minPlayers;

    [SerializeField] int maxPlayers;

    [SerializeField] List<MissionListEntry> list;

    /// <summary>
    /// Lowest playercount this mission list can appear in
    /// </summary>
    public int MinPlayers
    {
        get
        {
            return minPlayers;
        }
    }

    /// <summary>
    /// Highest playercount this mission list can appear in
    /// </summary>
    public int MaxPlayers
    {
        get
        {
            return maxPlayers;
        }
    }

    /// <summary>
    /// Missions in this list. First list is the order of the missions, second list is what missions can show up in that position.
    /// </summary>
    public List<MissionListEntry> List
    {
        get
        {
            return list;
        }
    }
}

/// <summary>
/// Stupid way of getting around Unity's serializing system.
/// </summary>
[System.Serializable]
public struct MissionListEntry
{
    /// <summary>
    /// Private counterpart of <see cref="Missions"/>
    /// </summary>
    [SerializeField] List<MissionListEntryEntry> missions;

    /// <summary>
    /// Each entry in the mission list is an array of missions that can show up at that position
    /// </summary>
    public List<MissionListEntryEntry> Missions
    {
        get
        {
            return missions;
        }
        set
        {
            missions = Missions;
        }
    }
}

/// <summary>
/// Stupid way of getting around Unity's serializing system.
/// </summary>
[System.Serializable]
public struct MissionListEntryEntry
{
    /// <summary>
    /// Private counterpart of <see cref="Mission"/>
    /// </summary>
    [SerializeField] Mission mission;

    /// <summary>
    /// Private counterpart of <see cref="Weight"/>
    /// </summary>
    [SerializeField] float weight;

    /// <summary>
    /// The mission
    /// </summary>
    public Mission Mission
    {
        get
        {
            return mission;
        }
        set
        {
            mission = Mission;
        }
    }

    /// <summary>
    /// How likely the mission is to show up
    /// </summary>
    public float Weight
    {
        get
        {
            return weight;
        }
        set
        {
            weight = Weight;
        }
    }
}
