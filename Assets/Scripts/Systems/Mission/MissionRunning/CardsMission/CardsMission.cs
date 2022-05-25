using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardsMission : MissionType
{
    /// <summary>
    /// The card info for each player
    /// </summary>
    public Dictionary<Player,Deck> CardInfo;

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
    private List<Player> playersPlayed;


    [SerializeField] Setup setup;

    /// <summary>
    /// Invokes when any player draws a card
    /// </summary>
    public event DrawCard OnDrawCard;
    public delegate void DrawCard(Card card);

    
    protected override void Start()
    {
        base.Start();
        CardInfo = new Dictionary<Player, Deck>();
        setup.OnGamePhaseEnd += OnSetupFinished;
        NetworkServer.RegisterHandler<DrawCardMsg>(PlayerClickedDraw);
        NetworkServer.RegisterHandler<PlayerPlayedMsg>(PlayerClickedSubmit);
    }

    void OnSetupFinished()
    {
        foreach (Role role in GameInfo.Roles)
        {
            List<Card> playerDeck = new List<Card>();
            for (int i = 0; i < role.Data.StartingDeck.Count; i++)
            {
                playerDeck.Add(new Card(role.Data.StartingDeck[i]));
            }
            Deck deck = new Deck(playerDeck);
            deck.Shuffle();

            CardInfo.Add(role.Ability.Owner, deck);
        }
    }

    public override void StartMission()
    {
        base.StartMission();
        playedTotal = 0;
        playersPlayed = new List<Player>();
        NetworkServer.SendToAll(new CardMissionStartedMsg() { });
    }

    private void PlayerClickedDraw(NetworkConnection conn, DrawCardMsg msg)
    {
        if (!GameInfo.Players.TryGetValue(conn, out Player ply)) return;
        //If the player isn't on the mission
        if (!GameInfo.PlayersOnMission.Contains(ply)) return;
        CardInfo.TryGetValue(ply, out Deck deck);

        if (deck.Hand.Count > 0)
        {
            Card handCard = deck.Hand[0];
            deck.Discard(handCard);
        }

        deck.Draw();

        if (deck.Hand.Count == 0) return;

        OnDrawCard?.Invoke(deck.Hand[0]);

        conn.Send(new DrawCardMsg()
        {
            drawnCard = deck.Hand[0]
        });
    }

    private void PlayerClickedSubmit(NetworkConnection conn, PlayerPlayedMsg msg)
    {
        if (!GameInfo.Players.TryGetValue(conn, out Player ply)) return;
        //If the player isn't on the mission
        if (!GameInfo.PlayersOnMission.Contains(ply)) return;
        //If the player has already played a card
        if (playersPlayed.Contains(ply)) return;
        CardInfo.TryGetValue(ply, out Deck deck);

        Card card = deck.Play();

        playedTotal += card.Value;

        playersPlayed.Add(ply);

        if (playersPlayed.Count >= GameInfo.PlayersOnMission.Count)
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
        foreach (KeyValuePair<Player, Deck> deck in CardInfo)
        {
            int result = 0;
            //Only display the total of all cards played, not how many they've played
            for (int i = 0; i < deck.Value.Played.Count; i++)
            {
                result += deck.Value.Played[i].Value;
            }
            finalCards.Add(result);
        }


        foreach (KeyValuePair<NetworkConnection, Player> pair in GameInfo.Players)
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