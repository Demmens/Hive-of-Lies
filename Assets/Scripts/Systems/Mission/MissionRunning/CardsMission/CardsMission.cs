using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardsMission : MissionType
{
    /// <summary>
    /// The difficulty of the mission
    /// </summary>
    public static int Difficulty = 25;

    /// <summary>
    /// The penalty given to a players card result when they're exhausted
    /// </summary>
    public static int ExhaustionPenalty = 5;

    /// <summary>
    /// Total value of all played cards
    /// </summary>
    private int playedTotal = 0;

    /// <summary>
    /// List of all players who have played cards
    /// </summary>
    private List<HoLPlayer> playersPlayed;

    [Tooltip("All the roles in the game")]
    [SerializeField] RoleSet allRoles;

    [Tooltip("All the players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;

    [Tooltip("All players by their NetworkConnection")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("Set of all players on the mission")]
    [SerializeField] HoLPlayerSet playersOnMission;

    [Tooltip("Number of draws the player gets to make for free. Should always be at least 1.")]
    [SerializeField] IntVariable freeDraws;

    [Tooltip("Cost of the first paid draw.")]
    [SerializeField] IntVariable firstDrawCost;

    [Tooltip("How much to increase all draw costs by")]
    [SerializeField] IntVariable globalDrawCostMod;

    /// <summary>
    /// Invokes when any player draws a card
    /// </summary>
    public event CardDelegate OnDrawCard;
    public delegate void CardDelegate(HoLPlayer ply, ref Card card);

    public event DrawnCardDelegate OnPlayCard;
    public delegate void DrawnCardDelegate(HoLPlayer ply, ref Card card, ref int value);

    public void AfterRolesChosen()
    {
        foreach (Role role in allRoles.Value)
        {
            HoLPlayer ply = role.Ability.Owner;
            for (int i = 0; i < role.Data.StartingDeck.Count; i++)
            {
                ply.Deck.Value.DrawPile.Add(new Card(role.Data.StartingDeck[i]));
            }
            ply.Deck.Value.Shuffle();
        }
    }

    public override void StartMission()
    {
        playedTotal = 0;
        playersPlayed = new List<HoLPlayer>();
        base.StartMission();
    }

    public void PlayerClickedDraw(NetworkConnection conn)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        //If the player isn't on the mission
        if (!playersOnMission.Value.Contains(ply)) return;

        if (ply.Favour < ply.NextDrawCost && ply.NextDrawCost > 0) return;

        Deck deck = ply.Deck;

        if (deck.Hand.Count > 0)
        {
            Card handCard = deck.Hand[0];
            deck.Discard(handCard);
        }

        ply.Favour.Value -= ply.NextDrawCost;

        ply.NumDraws++;
        ply.NextDrawCost.Value = CalculateDrawCost(ply.NumDraws);

        deck.Draw();
    }

    public void PlayerClickedSubmit(NetworkConnection conn)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        //If the player isn't on the mission
        if (!playersOnMission.Value.Contains(ply)) return;
        //If the player has already played a card
        if (playersPlayed.Contains(ply)) return;
        Deck deck = ply.Deck;

        Card card = deck.Play();

        int value = card.Value;

        OnPlayCard?.Invoke(ply, ref card, ref value);

        playedTotal += card.Value;

        playersPlayed.Add(ply);

        if (playersPlayed.Count >= playersOnMission.Value.Count)
        {
            AllPlayersPlayed();
            NetworkServer.SendToAll(new PlayerPlayedMsg()
            {
                lastPlayer = true
            });
        }
        else
        {
            NetworkServer.SendToAll(new PlayerPlayedMsg()
            {
            });
        }
    }

    private void AllPlayersPlayed()
    {
        Debug.Log("All players have submitted");
        result = playedTotal >= Difficulty ? MissionResult.Success : MissionResult.Fail;

        List<int> finalCards = new List<int>();
        foreach (HoLPlayer ply in allPlayers.Value)
        {
            int result = 0;
            //Only display the total of all cards played, not how many they've played
            for (int i = 0; i < ply.Deck.Value.Played.Count; i++)
            {
                result += ply.Deck.Value.Played[i].Value;
            }
            finalCards.Add(result);
        }


        foreach (KeyValuePair<NetworkConnection, HoLPlayer> pair in playersByConnection.Value)
        {
            CreateMissionResultPopupMsg msg = new CreateMissionResultPopupMsg()
            {
                result = result,
            };

            if (playersPlayed.Contains(pair.Value)) msg.finalCards = finalCards;

            pair.Key.Send(msg);
        }
    }

    /// <summary>
    /// Calculates the cost of the draw
    /// </summary>
    /// <param name="ply">The player drawing a card</param>
    /// <param name="numRerolls">The number of cards they have drawn so far</param>
    /// <returns></returns>
    [Server]
    public int CalculateDrawCost(int numDraws)
    {
        //Make sure the correct number of rerolls are free
        if (numDraws < freeDraws) return 0;

        //Calculate how many rerolls we have used that aren't free.
        numDraws -= freeDraws;

        //Formula. Can edit this however we like for balance.
        int cost = firstDrawCost * (numDraws + 1);

        //Currently deciding to put this before roles tinker with it. Might change later, who knows.
        cost += globalDrawCostMod;

        return Mathf.Max(cost, 0);
    }
}

public struct DrawCardMsg : NetworkMessage
{
    public Card drawnCard;
}

public struct PlayerPlayedMsg : NetworkMessage
{
    public bool lastPlayer;
}

public struct AllPlayersPlayedMsg : NetworkMessage
{
    
}

public struct CardMissionStartedMsg : NetworkMessage
{

}

public struct CreateMissionResultPopupMsg : NetworkMessage
{
    public MissionResult result;
    public List<int> finalCards;
}