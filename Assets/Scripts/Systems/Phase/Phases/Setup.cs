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
    Dictionary<Team, int> RoleChoices = new()
    {
        {Team.Bee, 2},
        {Team.Wasp, 2}
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

    [SerializeField] FloatVariable popupX;
    [SerializeField] FloatVariable popupY;

    [SerializeField] Transform topPlayerRow;
    [SerializeField] Transform rightPlayerRow;
    [SerializeField] Transform bottomPlayerRow;
    [SerializeField] Transform leftPlayerRow;

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
        for (int i = 0; i < plys.Count; i++)
        {
            HoLPlayer ply = plys[i];
            GameObject button = Instantiate(playerButton);
            NetworkServer.Spawn(button);
            SetupButtonOnClient(button, i, plys.Count);
            PlayerButton plButton = button.GetComponent<PlayerButton>();
            plButton.Owner = ply;
            ply.Button = plButton;
        }
    }

    /// <summary>
    /// Place the player buttons in the correct positions around the table
    /// </summary>
    [ClientRpc]
    void SetupButtonOnClient(GameObject button, int currentPlayer, int maxPlayers)
    {
        // Think about: Should the clients player button always be in the top-middle position if possible?

        // Dictionary<Player Count, Num Seats (top, right, bottom, left)>
        Dictionary<int, (int, int, int, int)> seats = new()
        {
            { 1, (1,0,0,0) },
            { 2, (1,0,1,0) },
            { 3, (1,0,2,01) },
            { 4, (2,0,2,0) },
            { 5, (2,0,3,0) },
            { 6, (3,0,3,0) },
            { 7, (2,1,3,1) },
            { 8, (3,1,3,1) },
            { 9, (2,2,3,2) },
            {10, (3,2,3,2) },
            {11, (3,2,4,2) },
            {12, (4,2,4,2) },
        };

        if (!seats.TryGetValue(maxPlayers, out (int, int, int, int) seatingCounts)) return;

        int i = 0;

        i += seatingCounts.Item1;
        if (currentPlayer < i) 
        {
            button.transform.SetParent(topPlayerRow);
            return;
        }

        i += seatingCounts.Item2;
        if (currentPlayer < i)
        {
            Transform imageTransform = button.GetComponentInChildren<UnityEngine.UI.RawImage>().transform;

            imageTransform.localScale = new Vector3(-imageTransform.localScale.x, imageTransform.localScale.y, imageTransform.localScale.z);

            button.transform.SetParent(rightPlayerRow);
            return;
        }

        i += seatingCounts.Item3;
        if (currentPlayer < i)
        {
            button.transform.SetParent(bottomPlayerRow);
            return;
        }

        i += seatingCounts.Item4;
        if (currentPlayer < i)
        {
            button.transform.SetParent(leftPlayerRow);
            return;
        }

        Debug.LogError($"Not enough seats for the current player count ({maxPlayers})");
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
        if (team == Team.Wasp)
        {
            teamPopup.GetComponent<Notification>().SetText("<b>YOU ARE A WASP</b>\n\nStay hidden, sabotage missions, and find your target."); ;
        }
        else
        {
            string text = "<b>YOU ARE A BEE</b>\n\nSucceed missions, find allies, and don't let <b>anyone</b> know your role.";
            
            if (waspPlayers.Value.Count == 1)
            {
                text += "\nThere is 1 Wasp.";
            }
            else
            {
                text += $"\nThere are {waspPlayers.Value.Count} Wasps.";
            }
            teamPopup.GetComponent<Notification>().SetText(text);
        }
        
        float x = popupX.Value > 0 ? popupX : Screen.width/2;
        float y = popupY.Value > 0 ? popupY : Screen.height/2;
        teamPopup.transform.GetChild(0).position = new Vector3(x, y, 1);
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