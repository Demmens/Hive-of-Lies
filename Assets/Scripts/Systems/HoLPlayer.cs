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
    public TeamVariable Team = new TeamVariable();

    /// <summary>
    /// The game's currency. Used for redrawing cards, placing extra votes, and running for team leader.
    /// </summary>
    [HideInInspector]
    public IntVariable Favour = new IntVariable();

    /// <summary>
    /// The role that the player has. This determines their deck of cards, starting favour, and special ability.
    /// </summary>
    [HideInInspector]
    public RoleVariable Role = new RoleVariable();

    /// <summary>
    /// How many missions the player has been on in a row. Each level reduces the value of the cards in their deck.
    /// </summary>
    [HideInInspector]
    public IntVariable Exhaustion = new IntVariable();

    /// <summary>
    /// How much favour it costs to draw another card
    /// </summary>
    [HideInInspector]
    public IntVariable NextDrawCost = new IntVariable();

    /// <summary>
    /// How much favour it costs to place another vote
    /// </summary>
    [HideInInspector]
    public IntVariable NextVoteCost = new IntVariable();

    /// <summary>
    /// List of the choices of roles the player will have at the start of the game
    /// </summary>
    [HideInInspector]
    public List<RoleData> RoleChoices = new List<RoleData>();
    #endregion
}