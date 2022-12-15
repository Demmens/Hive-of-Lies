using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardsMission : MissionType
{
    public static CardsMission singleton;

    [Tooltip("The decks of all players")]
    [SerializeField] CardInfo cardInfo;

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

    [Tooltip("All players by their NetworkConnection")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("Set of all players on the mission")]
    [SerializeField] HoLPlayerSet playersOnMission;

    /// <summary>
    /// Invokes when any player draws a card
    /// </summary>
    public event CardDelegate OnDrawCard;
    public delegate void CardDelegate(HoLPlayer ply, ref Card card);

    public event DrawnCardDelegate OnPlayCard;
    public delegate void DrawnCardDelegate(HoLPlayer ply, ref Card card, ref int value);

    protected override void Start()
    {
        singleton = this;
        base.Start();
        cardInfo.Value = new Dictionary<HoLPlayer, Deck>();
        NetworkServer.RegisterHandler<DrawCardMsg>(PlayerClickedDraw);
        NetworkServer.RegisterHandler<PlayerPlayedMsg>(PlayerClickedSubmit);
    }

    public void OnSetupFinished()
    {
        foreach (Role role in allRoles.Value)
        {
            List<Card> playerDeck = new List<Card>();
            for (int i = 0; i < role.Data.StartingDeck.Count; i++)
            {
                playerDeck.Add(new Card(role.Data.StartingDeck[i]));
            }
            Deck deck = new Deck(playerDeck);
            deck.Shuffle();

            cardInfo.Value.Add(role.Ability.Owner, deck);
        }
    }

    public override void StartMission()
    {
        base.StartMission();
        playedTotal = 0;
        playersPlayed = new List<HoLPlayer>();
        NetworkServer.SendToAll(new CardMissionStartedMsg() { });
    }

    private void PlayerClickedDraw(NetworkConnection conn, DrawCardMsg msg)
    {
        if (playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        //If the player isn't on the mission
        if (!playersOnMission.Value.Contains(ply)) return;
        cardInfo.Value.TryGetValue(ply, out Deck deck);

        if (deck.Hand.Count > 0)
        {
            Card handCard = deck.Hand[0];
            deck.Discard(handCard);
        }

        deck.Draw();

        if (deck.Hand.Count == 0) return;

        Card drawnCard = deck.Hand[0];

        OnDrawCard?.Invoke(ply, ref drawnCard);

        conn.Send(new DrawCardMsg()
        {
            drawnCard = deck.Hand[0]
        });
    }

    private void PlayerClickedSubmit(NetworkConnection conn, PlayerPlayedMsg msg)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        //If the player isn't on the mission
        if (!playersOnMission.Value.Contains(ply)) return;
        //If the player has already played a card
        if (playersPlayed.Contains(ply)) return;
        cardInfo.Value.TryGetValue(ply, out Deck deck);

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
        foreach (KeyValuePair<HoLPlayer, Deck> deck in cardInfo.Value)
        {
            int result = 0;
            //Only display the total of all cards played, not how many they've played
            for (int i = 0; i < deck.Value.Played.Count; i++)
            {
                result += deck.Value.Played[i].Value;
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