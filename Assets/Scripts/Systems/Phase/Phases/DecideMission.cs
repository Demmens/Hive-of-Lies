using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Decides what the next mission will be
/// </summary>
public class DecideMission : GamePhase
{

    /// <summary>
    /// List of all possible mission lists for all possible player counts.
    /// </summary>
    [SerializeField] List<MissionList> missionLists;

    /// <summary>
    /// Which players have voted
    /// </summary>
    List<HoLPlayer> TotalVotes;

    /// <summary>
    /// The mission that was voted in
    /// </summary>
    Mission DecidedMission;

    [Tooltip("The playercount of the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("The current round number of the game")]
    [SerializeField] IntVariable roundNum;

    [Tooltip("The number of mission choices the players have")]
    [SerializeField] IntVariable numMissionChoices;

    [Tooltip("The mission list that will be used this game")]
    [SerializeField] MissionListVariable decidedMissionList;

    [Tooltip("All players by their connection")]
    [SerializeField] HoLPlayerDictionary players;

    [Tooltip("The currently active mission")]
    [SerializeField] MissionVariable currentMission;

    [HideInInspector] public List<Mission> CompletedMissions = new List<Mission>();

    /// <summary>
    /// Who has voted for which mission, and how many votes in total that mission has
    /// </summary>
    public Dictionary<Mission, (List<HoLPlayer>, int)> MissionVotes;

    #region Events

    /// <summary>
    /// Delegate for <see cref="BeforeMissionResult"/>
    /// </summary>
    /// <param name="missionVotes"></param>
    public delegate void BeforeMissionResultDelegate();
    public event BeforeMissionResultDelegate BeforeMissionResult;

    #endregion

    void Start()
    {
        //Listen for player voting
        NetworkServer.RegisterHandler<PlayerVotedOnMissionMsg>(PlayerVoted);

        //Keep track of all the lists we find that are valid for this game
        List<MissionList> possibleLists = new List<MissionList>();

        missionLists.ForEach(list =>
        {
            //If the current playercount falls within the valid range for the mission list
            if (list.MinPlayers <= playerCount && list.MaxPlayers >= playerCount)
            {
                possibleLists.Add(list);
            }
        });

        decidedMissionList.Value = possibleLists.GetRandom();
    }

    /// <summary>
    /// Determine what the mission choices given to the players should be
    /// </summary>
    public override void Begin()
    {
        TotalVotes = new List<HoLPlayer>();
        MissionVotes = new Dictionary<Mission, (List<HoLPlayer>, int)>();

        //List of missions and weights. Make sure if we reach the end of the list that we just start looping the final mission indefinitely.
        int missionListIndex = Mathf.Min(roundNum, decidedMissionList.Value.List.Count - 1);
        List<MissionListEntryEntry> missionDataChoices = decidedMissionList.Value.List[missionListIndex].Missions;

        //Essentially the same as above, but we can edit it as much as we like since it's non-static.
        List<(Mission,float)> missionChoices = new List<(Mission,float)>();

        //Pure list of missions to send to clients
        List<Mission> choices = new List<Mission>();


        // Find {MissionChoices} random missions from the list of missions given their particular weightings.
        // Useful to imagine as taking balls out of a bag and each mission is a different colour of ball.
        // The number of balls in the bag of any given colour is equal to the weighting of the mission.
        

        //Total Weight
        float total = 0;

        //Find the total weight of all objects so we know how many balls are in the bag
        missionDataChoices.ForEach(miss =>
        {
            //Only add missions if we meet the condition
            if (miss.Mission.Condition.Condition())
            {
                total += miss.Weight;
                missionChoices.Add((miss.Mission, total)); //Set each weight to be cumulative so we can find actual probabilities later.
            }
        });

        //Draw a ball from the bag and then remove all instances of that colour of ball so we don't draw the same mission twice.
        for (int i = 0; i < numMissionChoices; i++)
        {
            float rand = 1;
            while (rand == 1) rand = Random.Range(0f, 1f); //Get a random float in the range [0,1).

            // Weight of the selected mission. Used to reduce weight of missions above it in the list to compensate for removal.
            float selectedWeight = 0;

            //Run for each mission choice
            for (int j = 0; j < missionChoices.Count; j++)
            {
                (Mission,float) miss = missionChoices[j];
                //Make sure we only select one item per pass
                if (selectedWeight == 0)
                {
                    //Check probabilities of each item
                    if (rand < miss.Item2 / total)
                    {
                        //Add the mission to the selection
                        MissionVotes.Add(miss.Item1, (new List<HoLPlayer>(), 0));
                        choices.Add(miss.Item1);
                        //Remove all instances of this mission, thus reducing the total number of balls in the bag.
                        total -= miss.Item2;
                        //Store the weight of this mission so we can adjust the weight of the other missions to compensate for this missions removal.
                        selectedWeight = miss.Item2;
                        //Make sure this item can't come up again
                        missionChoices.RemoveAt(j);
                        j--;
                    }
                }  
                else
                {
                    //Reduce weight to compensate for a mission below this in the list being picked.
                    miss.Item2 -= selectedWeight;
                    missionChoices[j] = miss;
                }
            }
        }

        NetworkServer.SendToAll(new SendMissionChoicesMsg
        {
            choices = choices
        });
    }

    /// <summary>
    /// Call when a player votes to count it.
    /// </summary>
    /// <param name="ply">The player that voted</param>
    /// <param name="mission">The mission they voted for</param>
    public void PlayerVoted(NetworkConnection conn, PlayerVotedOnMissionMsg msg)
    {
        if (!Active) return;

        (List<HoLPlayer>, int) Tuple;
        players.Value.TryGetValue(conn, out HoLPlayer ply);

        //Make sure players don't vote twice.
        if (TotalVotes.Contains(ply)) return;

        //Make sure they're voting on a valid mission.
        if (!MissionVotes.TryGetValue(msg.mission, out Tuple)) return;

        Tuple.Item1.Add(ply);
        Tuple.Item2++;

        MissionVotes[msg.mission] = Tuple;
        TotalVotes.Add(ply);

        if (TotalVotes.Count >= playerCount)
        {
            //Let everyone know we're just about to determine the mission result.
            //Allows other classes to quickly change the info
            BeforeMissionResult?.Invoke();

            DetermineMission();
        }
    }

    /// <summary>
    /// Determine which mission won the vote. This becomes the active mission
    /// </summary>
    void DetermineMission()
    {
        int maxVotes = int.MinValue;
        foreach (KeyValuePair<Mission,(List<HoLPlayer>,int)> item in MissionVotes)
        {
            if (item.Value.Item2 > maxVotes)
            {
                maxVotes = item.Value.Item2;

                DecidedMission = item.Key;
            }
        }

        currentMission.Value = DecidedMission;
        NetworkServer.SendToAll(new SendDecidedMissionMsg()
        {
            mission = DecidedMission
        });

        End();
    }
}

public struct PlayerVotedOnMissionMsg : NetworkMessage
{
    public Mission mission;
}

public struct SendMissionChoicesMsg : NetworkMessage
{
    public List<Mission> choices;
}

public struct SendDecidedMissionMsg : NetworkMessage
{
    public Mission mission;
}