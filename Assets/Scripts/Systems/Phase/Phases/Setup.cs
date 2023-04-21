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

    [Tooltip("All the roles that can appear in the game")]
    [SerializeField] RoleDataSet roles;

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

    [SerializeField] GameObject teamPopup;

    [SerializeField] GameObject playerButton;
    [SerializeField] Transform playerList;

    /// <summary>
    /// Run the game setup. This includes handing out roles and selecting teams.
    /// </summary>
    [Server]
    public void BeginSetup()
    {
        Debug.Log("All players have entered the game. Beginning setup.");

        roles.Value.Shuffle();
        //Shuffle the players so we can randomly assign teams
        allPlayers.Value.Shuffle();
        //Shuffle the roles so we can randomly dish them out to players
        //Roles.Shuffle();
        CreateButtons(allPlayers);

        AssignTeams(allPlayers);

        GiveRoleChoices(allPlayers);

        setupFinished?.Invoke();
        Debug.Log("Setup Finished");
    }

    void CreateButtons(List<HoLPlayer> plys)
    {
        foreach (HoLPlayer ply in plys)
        {
            GameObject button = Instantiate(playerButton);
            NetworkServer.Spawn(button);
            SetupButtonOnClient(button);
            PlayerButton plButton = button.GetComponent<PlayerButton>();
            plButton.Owner = ply;
            ply.Button = plButton;
        }
    }

    [ClientRpc]
    void SetupButtonOnClient(GameObject button)
    {
        button.transform.SetParent(playerList.transform);
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

            DisplayTeamPopup(ply.connectionToClient, ply.Team);
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
    void GiveRoleChoices(List<HoLPlayer> plys)
    {
        foreach (HoLPlayer ply in plys)
        {
            ply.RoleChoices = new();
            ply.Role.Value = null;
            RoleChoices.TryGetValue(ply.Team, out int choices);
            for (int i = 0; i < roles.Value.Count; i++)
            {
                RoleData role = roles.Value[i];
                if (role.Team != ply.Team) continue;
                if (!role.Enabled) continue;
                if (role.PlayersRequired > playerCount) continue;

                ply.RoleChoices.Add(role);
                roles.Remove(role);
                i--;
                if (ply.RoleChoices.Count == choices) break;
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