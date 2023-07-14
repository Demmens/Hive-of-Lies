using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionList", menuName = "Missions/Create mission list")]
public class MissionList : ScriptableObject
{
    [SerializeField] int minPlayers;

    [SerializeField] int maxPlayers;

    [Tooltip("Extra mission lists to be automatically included in this list")]
    public List<MissionList> IncludedThreads = new();

    /// <summary>
    /// Missions in this list. First list is the order of the missions, second list is what missions can show up in that position.
    /// </summary>
    public List<MissionListEntry> List = new();

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

    //Include the included threads missions in this mission list
    public void AddThreads(MissionList origin = null, bool ignoreOriginSize = false)
    {
        if (origin == this) throw new System.Exception($"Mission list thread from origin {origin.name} is cyclic");
        if (origin == null)
        {
            origin = this;
            Debug.Log($"Calling AddThreads on {name}");
        }

        foreach (MissionList t in IncludedThreads)
        {
            Debug.Log($"Adding thread: {t}");
            //Duplicate the thread so we can make changes to it if necessary
            MissionList thread = Instantiate(t);
            thread.AddThreads(origin);
            for (int i = 0; i < thread.List.Count && (i < origin.List.Count || !ignoreOriginSize); i++)
            {
                if (origin.List.Count == i) origin.List.Add(new MissionListEntry() { Missions = new() });
                MissionListEntry entry = origin.List[i];
                MissionListEntry threadEntry = thread.List[i];
                //entry.Missions.AddRange(threadEntry.Missions);
                foreach (MissionListEntryEntry mission in entry.Missions)
                {
                    threadEntry.Missions.Add(mission);
                }
            }
        }

        if (origin == this)
        {
            foreach (MissionListEntry entry in List)
            {
                foreach (MissionListEntryEntry mission in entry.Missions)
                {
                    mission.Mission.DifficultyMod = 0;
                }
            }
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
    /// Each entry in the mission list is an array of missions that can show up at that position
    /// </summary>
    public List<MissionListEntryEntry> Missions;
}

/// <summary>
/// Stupid way of getting around Unity's serializing system.
/// </summary>
[System.Serializable]
public struct MissionListEntryEntry
{
    /// <summary>
    /// The mission
    /// </summary>
    public Mission Mission;

    /// <summary>
    /// How likely the mission is to show up
    /// </summary>
    public float Weight;
}
