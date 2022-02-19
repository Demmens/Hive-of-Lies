using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class Setup : GamePhase
{
    public override EGamePhase Phase {
        get
        {
            return EGamePhase.Setup;
        }
    }

    /// <summary>
    /// Reference to the GameInfo class
    /// </summary>
    [SerializeField] GameInfo info;

    /// <summary>
    /// Number of role choices given to each role. If set too high, some players may not receive any choices.
    /// </summary>
    [SerializeField] Dictionary<Team, int> RoleChoices = new Dictionary<Team, int>()
    {
        {Team.Bee, 3},
        {Team.Wasp, 3}
    };

    /// <summary>
    /// Percentage of players that will be a traitor
    /// </summary>
    [SerializeField] float TraitorRatio;

    /// <summary>
    /// Private counterpart to <see cref="Roles"/>
    /// </summary>
    [SerializeField] List<RoleData> roles;

    /// <summary>
    /// Temporary list of all players in the game
    /// </summary>
    List<Player> players = new List<Player>();

    /// <summary>
    /// List of all roles that can appear in the game
    /// </summary>
    public List<RoleData> Roles
    {
        get
        {
            return roles;
        }
        set
        {
            roles = Roles;
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
    /// <param name="conn">The connection of the player</param>
    /// <param name="msg">The message</param>
    void OnPlayerReady(NetworkConnection conn, PlayerReadyMsg msg)
    {
        Debug.Log($"{SteamFriends.GetFriendPersonaName(msg.playerID)} loaded into the game");
        Player ply = new Player(conn, msg.playerID);

        players.Add(ply);
        GameInfo.Players.Add(conn, ply);

        //Begin the setup once all players are in.
        if (players.Count == GameInfo.PlayerCount)
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
        players.Shuffle();
        //Shuffle the roles so we can randomly dish them out to players
        //Roles.Shuffle();

        AssignTeams(players);
        
        GiveRoleChoices(players, Roles);
    }

    /// <summary>
    /// Assign a team to each player
    /// </summary>
    void AssignTeams(List<Player> plys)
    {
        //Increments to determine whether a player should be an innocent or a traitor
        float teamCounter = 0;
        plys.ForEach(ply =>
        {
            teamCounter += TraitorRatio;
            if (teamCounter >= 1)
            {
                teamCounter--;
                ply.Team = Team.Wasp;
            }
            else
            {
                ply.Team = Team.Bee;
            }
        });
    }

    /// <summary>
    /// Give players a selection of roles to choose from
    /// </summary>
    void GiveRoleChoices(List<Player> plys, List<RoleData> roles)
    {
        foreach (Player ply in plys)
        {
            RoleChoices.TryGetValue(ply.Team, out int choices);
            for (int i = 0; i < roles.Count; i++)
            {
                RoleData role = roles[i];
                if (role.Team == ply.Team)
                {
                    ply.RoleChoices.Add(role);
                    roles.Remove(role);
                    i--;
                    if (ply.RoleChoices.Count == choices) break;
                }
            }
        }

        foreach (Player ply in plys)
        {
            ply.Conn.Send(new SendRoleInfoMsg
            {
                roleChoices = ply.RoleChoices,
            });
        }
    }

    /// <summary>
    /// Called when a player selects their role
    /// </summary>
    /// <param name="ply">The player who selected the role</param>
    /// <param name="role">The role the player selected</param>
    public void PlayerSelectedRole(NetworkConnection conn, PlayerSelectedRoleMsg msg)
    {
        RoleData role = msg.role;
        if (!GameInfo.Players.TryGetValue(conn, out Player ply))
        {
            Debug.Log("Something went wrong. Couldn't find player from network connection.");
        }

        RoleAbility ability = Instantiate(role.Ability);
        ability.Owner = ply;
        ply.Favour = role.StartingFavour;

        info.Roles.Add(new Role()
        {
            Ability = ability,
            Data = role
        });

        if (info.Roles.Count == GameInfo.PlayerCount) End();
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
}

public struct PlayerSelectedRoleMsg : NetworkMessage
{
    public RoleData role;
}