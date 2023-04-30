using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

/// <summary>
/// Allows the TeamLeader to choose players to join them on the mission.
/// </summary>
public class TeamLeaderPickPartners : GamePhase
{
    /// <summary>
    /// Number of players that go on the mission with the TeamLeader for each player count
    /// </summary>
    readonly Dictionary<int, int> partnerPlayerCounts = new Dictionary<int, int>()
    {
        {0,1},
        {7,2},
        {10,3},
    };

    [Tooltip("Number of players that go on the mission with the TeamLeader in this game.")]
    [SerializeField] IntVariable numPartners;

    [Tooltip("Number of players in the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("The modifier of the mission difficulty")]
    [SerializeField] IntVariable missionDifficulty;

    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;

    [Tooltip("List of players the TeamLeader has selected so far")]
    [SerializeField] HoLPlayerSet playersSelected;

    [Tooltip("List of all players by their NetworkConnection")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("List of all players")]
    [SerializeField] HoLPlayerSet players;

    [Tooltip("List of all players on the mission")]
    [SerializeField] HoLPlayerSet playersOnMission;

    [Tooltip("Invoked at the start of this game phase")]
    [SerializeField] GameEvent teamLeaderCanPick;

    [Tooltip("Invoked when the team leader has locked in their choices for partners")]
    [SerializeField] GameEvent partnerChoicesLocked;

    [Tooltip("The button you need to click in order to lock in your choices")]
    [SerializeField] GameObject lockInButton;

    [Tooltip("The dropdown prefab to add a player to the mission")]
    [SerializeField] GameObject pickPlayerButton;

    [Tooltip("The dropdown prefab to remove a player from the mission")]
    [SerializeField] GameObject unpickPlayerButton;

    List<PlayerButtonDropdownItem> addItems = new();
    List<PlayerButtonDropdownItem> removeItems = new();
    private const int numPlayersForLockIn = 1;

    void Start()
    {
        //Find the appropriate number of players that need to go on each mission.
        for (int i = 0; i <= playerCount; i++)
        {
            if (partnerPlayerCounts.TryGetValue(i, out int num)) numPartners.Value = num;
        }

        missionDifficulty.Value += numPartners.Value - 1;
        
        if (NetworkClient.active)
        {
            lockInButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LockInChoices());
        }
    }

    [Server]
    public override void Begin()
    {
        playersSelected.Value = new();

        foreach (PlayerButtonDropdownItem i in addItems) Destroy(i);
        foreach (PlayerButtonDropdownItem i in removeItems) Destroy(i);

        foreach (HoLPlayer ply in players.Value)
        {
            CreateAddItem(ply);
        }

        teamLeaderCanPick?.Invoke();
    }

    public void OnServerConnected(NetworkConnection conn)
    {
        if (!Active) return;
        if (conn != teamLeader.Value.connectionToClient) return;

        foreach (PlayerButtonDropdownItem i in addItems) Destroy(i);
        foreach (PlayerButtonDropdownItem i in removeItems) Destroy(i);

        foreach (HoLPlayer ply in players.Value)
        {
            if (playersSelected.Value.Contains(ply)) CreateRemoveItem(ply);
            else if (playersSelected.Value.Count < numPartners) CreateAddItem(ply);
        }

        if (playersSelected.Value.Count >= numPlayersForLockIn) SetLockInActive(teamLeader.Value.connectionToClient, true);
    }

    [Server]
    void AddPlayer(HoLPlayer ply, PlayerButtonDropdownItem item)
    {
        if (playersSelected.Value.Count >= numPartners) return;
        if (playersSelected.Value.Contains(ply)) return;

        Destroy(item);
        addItems.Remove(item);
        CreateRemoveItem(ply);

        Debug.Log($"{teamLeader.Value.DisplayName} has selected {ply.DisplayName}");
        playersSelected.Add(ply);

        if (playersSelected.Value.Count >= numPlayersForLockIn) SetLockInActive(teamLeader.Value.connectionToClient, true);

        if (playersSelected.Value.Count < numPartners) return;

        OnMaxPlayersAdded();
    }

    [Server]
    void OnMaxPlayersAdded()
    {
        foreach (PlayerButtonDropdownItem i in addItems)
        {
            Destroy(i);
        }
    }

    [Server]
    void RemovePlayer(HoLPlayer ply, PlayerButtonDropdownItem item)
    {
        if (!playersSelected.Value.Contains(ply)) return;

        Destroy(item);
        CreateAddItem(ply);

        Debug.Log($"{teamLeader.Value.DisplayName} has deselected {ply.DisplayName}");
        playersSelected.Remove(ply);

        if (playersSelected.Value.Count < numPlayersForLockIn) SetLockInActive(teamLeader.Value.connectionToClient, false);

        if (playersSelected.Value.Count < numPartners - 1) return;

        OnNoLongerMaxPlayersAdded(ply);
    }

    [Server]
    void OnNoLongerMaxPlayersAdded(HoLPlayer ply)
    {
        foreach (HoLPlayer pl in players.Value)
        {
            if (playersSelected.Value.Contains(pl)) continue;
            //If it's the person we've just removed, then the add to mission button is created elsewhere
            if (pl == ply) continue;

            CreateAddItem(pl);
        }
    }

    /// <summary>
    /// Called when the TeamLeader chooses a player to join them
    /// </summary>
    /// <param name="conn">The connection of the player that the TeamLeader has selected</param>
    public void TeamLeaderSelectedPlayer(NetworkConnection conn)
    {
        if (!Active) return;
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        if (ply == teamLeader) return;

        if (!playersSelected.Value.Contains(ply))
        {
            if (playersSelected.Value.Count < numPartners)
            {
                Debug.Log($"{teamLeader.Value.DisplayName} has selected {ply.DisplayName}");
                playersSelected.Add(ply);
            }
        }
        else
        {
            playersSelected.Remove(ply);
        }
    }

    /// <summary>
    /// Called when the TeamLeader locks in their choices of players for the mission.
    /// </summary>
    [Command(requiresAuthority = false)]
    public void LockInChoices(NetworkConnectionToClient conn = null)
    {
        if (!Active) return;
        if (conn != teamLeader.Value.connectionToClient) return;

        foreach (PlayerButtonDropdownItem i in addItems) Destroy(i);
        foreach (PlayerButtonDropdownItem i in removeItems) Destroy(i);

        foreach (HoLPlayer ply in playersSelected.Value)
        {
            if (!playersOnMission.Value.Contains(ply)) playersOnMission.Value.Add(ply);
        };

        SetLockInActive(conn, false);

        End();
    }

    [Server]
    void CreateAddItem(HoLPlayer ply)
    {
        PlayerButtonDropdownItem item = ply.Button.AddDropdownItem(pickPlayerButton, teamLeader);
        item.OnItemClicked += (ply) => AddPlayer(ply, item);
        addItems.Add(item);
    }

    [Server]
    void CreateRemoveItem(HoLPlayer ply)
    {
        PlayerButtonDropdownItem item = ply.Button.AddDropdownItem(unpickPlayerButton, teamLeader);
        item.OnItemClicked += (ply) => RemovePlayer(ply, item);
        removeItems.Add(item);
    }

    [TargetRpc]
    void SetLockInActive(NetworkConnection conn, bool active)
    {
        lockInButton.SetActive(active);
    }
}