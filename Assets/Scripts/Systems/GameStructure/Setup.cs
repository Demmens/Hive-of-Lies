using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{

    [SerializeField] Player PlayerPrefab;

    /// <summary>
    /// Number of role choices given to each role. If set too high, some players may not receive any choices.
    /// </summary>
    [SerializeField] Dictionary<Team, int> RoleChoices = new Dictionary<Team, int>()
    {
        {Team.Innocent, 3},
        {Team.Traitor, 3}
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
    /// Temporary. Replace soon pls.
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


    /// <summary>
    /// Run the game setup. This includes handing out roles and selecting teams.
    /// </summary>
    public void RunSetup()
    {

        //Shuffle the players so we can randomly assign teams
        players.Shuffle();
        //Shuffle the roles so we can randomly dish them out to players
        Roles.Shuffle();

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
                ply.Team = Team.Traitor;
            }
            else
            {
                ply.Team = Team.Innocent;
            }
        });
    }

    /// <summary>
    /// Give players a selection of roles to choose from
    /// </summary>
    void GiveRoleChoices(List<Player> plys, List<RoleData> roles)
    {
        plys.ForEach(ply =>
        {
            if (RoleChoices.TryGetValue(ply.Team, out int choices))
            {
                int n = 0;
                List<RoleData> RoleChoices = new List<RoleData>();
                while (choices > 0 && n < roles.Count)
                {
                    if (roles[n].Team == ply.Team)
                    {
                        RoleChoices.Add(roles[n]);
                        roles.RemoveAt(n); // In future want to avoid removing values from this list, but it works for now.
                        choices--;
                    }
                    else n++;
                }

                //ply.DisplayRoleChoices(RoleChoices);
            }
        });
    }
}
