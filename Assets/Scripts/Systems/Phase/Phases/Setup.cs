using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class Setup : GamePhase
{

    /// <summary>
    /// Number of role choices given to each role. If set too high, some players may not receive any choices.
    /// </summary>
    [SerializeField]
    Dictionary<Team, int> RoleChoices = new Dictionary<Team, int>()
    {
        {Team.Bee, 2},
        {Team.Wasp, 2}
    };

    [Tooltip("The ratio of traitors to innocents")]
    [SerializeField] FloatVariable traitorRatio;

    /// <summary>
    /// Private counterpart to <see cref="Roles"/>
    /// </summary>
    [SerializeField] List<RoleData> roles;

    [Tooltip("Runtime set of all the players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;

    [Tooltip("Runtime set of all the wasp players in the game")]
    [SerializeField] HoLPlayerSet waspPlayers;

    [Tooltip("Runtime set of all the bee players in the game")]
    [SerializeField] HoLPlayerSet beePlayers;

    [Tooltip("The player count of the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("Dictionary of Players and their respective NetworkConnections")]
    [SerializeField] HoLPlayerDictionary playersByNetworkConnection;

    [Tooltip("Invoked when all the setup logic is completed")]
    [SerializeField] GameEvent setupFinished;

    [SerializeField] List<IntVariable> intVariablesToReset;

    [SerializeField] GameObject teamPopup;

    /// <summary>
    /// List of all roles that can appear in the game
    /// </summary>
    [SerializeField] List<RoleData> Roles
    {
        get
        {
            return roles;
        }
    }

    /// <summary>
    /// Run the game setup. This includes handing out roles and selecting teams.
    /// </summary>
    [Server]
    public void BeginSetup()
    {
        intVariablesToReset.ForEach(var => var.OnEnable());
        Debug.Log("All players have entered the game. Beginning setup.");

        Roles.Shuffle();
        //Shuffle the players so we can randomly assign teams
        allPlayers.Value.Shuffle();
        //Shuffle the roles so we can randomly dish them out to players
        //Roles.Shuffle();

        AssignTeams(allPlayers);

        GiveRoleChoices(allPlayers, Roles);

        setupFinished?.Invoke();
        Debug.Log("Setup Finished");
    }

    /// <summary>
    /// Assign a team to each player
    /// </summary>
    [Server]
    void AssignTeams(List<HoLPlayer> plys)
    {
        beePlayers.Value = new();
        waspPlayers.Value = new();
        //Increments to determine whether a player should be an innocent or a traitor
        float teamCounter = 0;
        plys.ForEach(ply =>
        {
            ply.IsAlive.Value = true;
            teamCounter += traitorRatio;
            if (teamCounter >= 1)
            {
                teamCounter--;
                ply.Team = ScriptableObject.CreateInstance<TeamVariable>();
                ply.Team.Value = Team.Wasp;
                waspPlayers.Add(ply);
            }
            else
            {
                ply.Team = ScriptableObject.CreateInstance<TeamVariable>();
                ply.Team.Value = Team.Bee;
                beePlayers.Add(ply);
            }

            DisplayTeamPopup(ply.Connection, ply.Team);
        });
    }

    [TargetRpc]
    void DisplayTeamPopup(NetworkConnection conn, Team team)
    {
        teamPopup = Instantiate(teamPopup);
        teamPopup.GetComponent<Notification>().SetText($"You are a {team}");
    }

    /// <summary>
    /// Give players a selection of roles to choose from
    /// </summary>
    [Server]
    void GiveRoleChoices(List<HoLPlayer> plys, List<RoleData> roles)
    {
        foreach (HoLPlayer ply in plys)
        {
            ply.RoleChoices = new();
            ply.Role.Value = null;
            RoleChoices.TryGetValue(ply.Team, out int choices);
            for (int i = 0; i < roles.Count; i++)
            {
                RoleData role = roles[i];
                if (role.Team == ply.Team && role.Enabled)
                {
                    ply.RoleChoices.Add(role);
                    roles.Remove(role);
                    i--;
                    if (ply.RoleChoices.Count == choices) break;
                }
            }
        }
    }

    /// <summary>
    /// We don't actually need the begin for this, since it begins under different circumstances.
    /// </summary>
    public override void Begin()
    {
    }
}