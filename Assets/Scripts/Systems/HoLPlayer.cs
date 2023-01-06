using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HoLPlayer : NetworkBehaviour
{
    #region Meta
    /// <summary>
    /// The ID associated with the player. This is currently derived from their CSteamID.
    /// </summary>
    public ulong PlayerID;

    /// <summary>
    /// The display name of this player
    /// </summary>
    public string DisplayName;

    /// <summary>
    /// Returns true on the client that is represented by this object.
    /// </summary>
    public bool LocalPlayer => hasAuthority && NetworkClient.active;

    /// <summary>
    /// The NetworkConnection associated with this player
    /// </summary>
    public NetworkConnection Connection;
    #endregion

    #region Gameplay
    /// <summary>
    /// The team of this player. Whether they are a bee or a wasp.
    /// </summary>
    public TeamVariable Team;

    /// <summary>
    /// The game's currency. Used for redrawing cards, placing extra votes, and running for team leader.
    /// </summary>
    public IntVariable Favour;

    /// <summary>
    /// The role that the player has. This determines their deck of cards, starting favour, and special ability.
    /// </summary>
    public RoleVariable Role;

    /// <summary>
    /// How many missions the player has been on in a row. Each level reduces the value of the cards in their deck.
    /// </summary>
    public IntVariable Exhaustion;

    /// <summary>
    /// How much favour it costs to draw another card
    /// </summary>
    public IntVariable NextDrawCost;

    /// <summary>
    /// How much favour it costs to place another upvote
    /// </summary>
    public IntVariable NextUpvoteCost;

    /// <summary>
    /// How much favour it costs to place another upvote
    /// </summary>
    public IntVariable NextDownvoteCost;

    /// <summary>
    /// How many votes this player is currently deciding to give
    /// </summary>
    public IntVariable NumVotes;

    /// <summary>
    /// The players deck of cards
    /// </summary>
    public DeckVariable Deck;

    /// <summary>
    /// How many times this player has drawn cards
    /// </summary>
    public IntVariable NumDraws;

    /// <summary>
    /// Whether this player is alive or not
    /// </summary>
    public BoolVariable IsAlive;

    /// <summary>
    /// The target of this player. Always null if the player is a bee.
    /// </summary>
    public HoLPlayerVariable Target;

    /// <summary>
    /// List of the choices of roles the player will have at the start of the game
    /// </summary>
    public List<RoleData> RoleChoices = new();
    #endregion

    /// <summary>
    /// Invoked when the players favour changes
    /// </summary>
    public event System.Action<NetworkConnection, int> OnFavourChanged;

    /// <summary>
    /// Invoked when the players upvote cost changes
    /// </summary>
    public event System.Action<NetworkConnection, int> OnUpvoteCostChanged;

    /// <summary>
    /// Invoked when the players downvote cost changes
    /// </summary>
    public event System.Action<NetworkConnection, int> OnDownvoteCostChanged;

    /// <summary>
    /// Invoked when the players downvote cost changes
    /// </summary>
    public event System.Action<NetworkConnection, int> OnNumVotesChanged;

    private void Awake()
    {
        Team = ScriptableObject.CreateInstance<TeamVariable>();
        Favour = ScriptableObject.CreateInstance<IntVariable>();
        Role = ScriptableObject.CreateInstance<RoleVariable>();
        Exhaustion = ScriptableObject.CreateInstance<IntVariable>();
        NextDrawCost = ScriptableObject.CreateInstance<IntVariable>();
        NextUpvoteCost = ScriptableObject.CreateInstance<IntVariable>();
        NextDownvoteCost = ScriptableObject.CreateInstance<IntVariable>();
        NumVotes = ScriptableObject.CreateInstance<IntVariable>();
        Deck = ScriptableObject.CreateInstance<DeckVariable>();
        NumDraws = ScriptableObject.CreateInstance<IntVariable>();
        IsAlive = ScriptableObject.CreateInstance<BoolVariable>();
        Target = ScriptableObject.CreateInstance<HoLPlayerVariable>();
        IsAlive.Value = true;
        Deck.Value = new();

        Favour.AfterVariableChanged += change => OnFavourChanged?.Invoke(Connection, change);
        Favour.OnVariableChanged += (int oldVal, ref int newVal) => newVal = Mathf.Max(0, newVal);
        NextUpvoteCost.AfterVariableChanged += change => OnUpvoteCostChanged?.Invoke(Connection, change);
        NextDownvoteCost.AfterVariableChanged += change => OnDownvoteCostChanged?.Invoke(Connection, change);
        NumVotes.AfterVariableChanged += change => OnNumVotesChanged?.Invoke(Connection, change);
    }
}

public enum Team
{
    Bee,
    Wasp,
    None,
}