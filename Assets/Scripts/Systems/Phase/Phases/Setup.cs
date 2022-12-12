using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class Setup : GamePhase
{
    public override EGamePhase Phase
    {
        get
        {
            return EGamePhase.Setup;
        }
    }

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

    /// <summary>
    /// Temporary list of all players in the game
    /// </summary>
    List<Player> players = new List<Player>();

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

    private void Start()
    {
        NetworkServer.RegisterHandler<PlayerReadyMsg>(OnPlayerReady);
        NetworkServer.RegisterHandler<PlayerSelectedRoleMsg>(PlayerSelectedRole);
    }

    /// <summary>
    /// Called when a player loads into the game
    /// </summary>
    void OnPlayerReady(NetworkConnection conn, PlayerReadyMsg msg)
    {
        Debug.Log($"{SteamFriends.GetFriendPersonaName(msg.playerID)} has loaded into the lobby");
        //If for whatever reason this is called twice on a client
        if (playerIDs.Contains(msg.playerID)) return;

        playerIDs.Add(msg.playerID);
        NetworkServer.SendToAll(new PlayerReadyMsg()
        {
            playerID = msg.playerID,
            loadedPlayers = playerIDs
        });

        Debug.Log($"{SteamFriends.GetFriendPersonaName(msg.playerID)} loaded into the game");
        HoLPlayer ply = new HoLPlayer(/*msg.playerID, conn*/);

        allPlayers.Add(ply);

        //Begin the setup once all players are in.
        if (allPlayers.Value.Count == playerCount)
        {
            Debug.Log("All players have entered the lobby. Beginning setup.");
            BeginSetup();
        }
    }

    /// <summary>
    /// Run the game setup. This includes handing out roles and selecting teams.
    /// </summary>
    public void BeginSetup()
    {
        Roles.Shuffle();
        //Shuffle the players so we can randomly assign teams
        allPlayers.Value.Shuffle();
        //Shuffle the roles so we can randomly dish them out to players
        //Roles.Shuffle();

        AssignTeams(allPlayers.Value);

        GiveRoleChoices(allPlayers, Roles);
    }

    /// <summary>
    /// Assign a team to each player
    /// </summary>
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
                ply.Team = new TeamVariable() { Value = Team.Wasp};
                waspPlayers.Add(ply);
            }
            else
            {
                ply.Team = new TeamVariable() { Value = Team.Bee };
                beePlayers.Add(ply);
            }
        });
    }

    /// <summary>
    /// Give players a selection of roles to choose from
    /// </summary>
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

        foreach (KeyValuePair<NetworkConnection, Player> pair in GameInfo.singleton.Players)
        {
            pair.Key.Send(new SendRoleInfoMsg
            {
                roleChoices = pair.Value.RoleChoices,
            });
        }
    }

    /// <summary>
    /// Called when a player selects their role
    /// </summary>
    public void PlayerSelectedRole(NetworkConnection conn, PlayerSelectedRoleMsg msg)
    {
        if (!Active) return;

        RoleData role = msg.role;
        playersByNetworkConnection.Value.TryGetValue(conn, out HoLPlayer ply);
        //If the role they selected is not one of their options
        if (!ply.RoleChoices.Contains(role)) return;
        GameObject abilityObject = Instantiate(role.Ability);
        RoleAbility ability = abilityObject.GetComponent<RoleAbility>();
        ability.Owner = ply;
        ability.OwnerConnection = conn;
        NetworkServer.Spawn(ability.gameObject, conn);
        ply.Favour.Value = role.StartingFavour;

        GameInfo.singleton.Roles.Add(new Role()
        {
            Ability = ability,
            Data = role
        });

        if (GameInfo.singleton.Roles.Count == GameInfo.singleton.PlayerCount) End();
    }

    /// <summary>
    /// We don't actually need the begin for this, since it begins under different circumstances.
    /// </summary>
    public override void Begin()
    {
    }
}

public struct SendRoleInfoMsg : NetworkMessage
{
    public List<RoleData> roleChoices;
}

public struct PlayerReadyMsg : NetworkMessage
{
    public CSteamID playerID;
    public List<CSteamID> loadedPlayers;
}

public struct PlayerSelectedRoleMsg : NetworkMessage
{
    public RoleData role;
}