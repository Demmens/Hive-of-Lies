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
        {Team.Bee, 3},
        {Team.Wasp, 3}
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

    [Tooltip("Must be a prefab containing a HoLPlayer script")]
    [SerializeField] GameObject playerObject;

    /// <summary>
    /// The steam IDs of all players currently in the lobby. Used to sync player buttons with clients
    /// </summary>
    List<CSteamID> playerIDs = new List<CSteamID>();

    /// <summary>
    /// List of all roles that can appear in the game
    /// </summary>
    public List<RoleData> Roles
    {
        get
        {
            return roles;
        }
    }

    /// <summary>
    /// Logic to execute when a player loads into the game
    /// </summary>
    [Server]
    public void OnPlayerReady(NetworkConnection conn)
    {
        //If for whatever reason this is called twice on a client
        if (playersByNetworkConnection.Value.TryGetValue(conn, out HoLPlayer pl)) return;

        GameObject playerObj = Instantiate(playerObject);
        NetworkServer.Spawn(playerObj, conn);
        HoLPlayer ply = playerObj.GetComponent<HoLPlayer>();
        ply.Connection = conn;
        ply.DisplayName = "DemTest";

        allPlayers.Add(ply);
        playersByNetworkConnection.Value[conn] = ply;
    }

    /// <summary>
    /// Run the game setup. This includes handing out roles and selecting teams.
    /// </summary>
    [Server]
    public void BeginSetup()
    {
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
        //Increments to determine whether a player should be an innocent or a traitor
        float teamCounter = 0;
        plys.ForEach(ply =>
        {
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
        });
    }

    /// <summary>
    /// Give players a selection of roles to choose from
    /// </summary>
    [Server]
    void GiveRoleChoices(List<HoLPlayer> plys, List<RoleData> roles)
    {
        foreach (HoLPlayer ply in plys)
        {
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