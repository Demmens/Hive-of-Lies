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
    [HideInInspector]
    public TeamVariable Team;

    /// <summary>
    /// The game's currency. Used for redrawing cards, placing extra votes, and running for team leader.
    /// </summary>
    [HideInInspector]
    public IntVariable Favour;

    /// <summary>
    /// The role that the player has. This determines their deck of cards, starting favour, and special ability.
    /// </summary>
    [HideInInspector]
    public RoleVariable Role;

    /// <summary>
    /// How many missions the player has been on in a row. Each level reduces the value of the cards in their deck.
    /// </summary>
    [HideInInspector]
    public IntVariable Exhaustion;

    /// <summary>
    /// How much favour it costs to draw another card
    /// </summary>
    [HideInInspector]
    public IntVariable NextDrawCost;

    /// <summary>
    /// How much favour it costs to place another vote
    /// </summary>
    [HideInInspector]
    public IntVariable NextVoteCost;

    /// <summary>
    /// List of the choices of roles the player will have at the start of the game
    /// </summary>
    [HideInInspector]
    public List<RoleData> RoleChoices = new List<RoleData>();
    #endregion

    public event System.Action<NetworkConnection, int> OnFavourChanged;

    private void Awake()
    {
        Team = ScriptableObject.CreateInstance<TeamVariable>();
        Favour = ScriptableObject.CreateInstance<IntVariable>();
        Role = ScriptableObject.CreateInstance<RoleVariable>();
        Exhaustion = ScriptableObject.CreateInstance<IntVariable>();
        NextDrawCost = ScriptableObject.CreateInstance<IntVariable>();
        NextVoteCost = ScriptableObject.CreateInstance<IntVariable>();

        Favour.AfterVariableChanged += (change) => OnFavourChanged?.Invoke(Connection, change);
    }


}