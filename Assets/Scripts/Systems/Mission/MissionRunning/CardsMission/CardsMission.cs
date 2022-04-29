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
    public int Difficulty = 25;

    /// <summary>
    /// The penalty given to a players card result when they're exhausted
    /// </summary>
    public int ExhaustionPenalty = 5;

    /// <summary>
    /// Total value of all played cards
    /// </summary>
    private int playedTotal = 0;

    /// <summary>
    /// List of all players who have played cards
    /// </summary>
    private List<Player> playersPlayed;


    [SerializeField] Setup setup;
    
    void Start()
    {
        CardInfo = new Dictionary<Player, Deck>();
        setup.OnGamePhaseEnd += OnSetupFinished;
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

            CardInfo.Add(role.Ability.Owner, deck);
        }
    }

    public override void StartMission()
    {
        base.StartMission();
        playedTotal = 0;
        playersPlayed = new List<Player>();
        NetworkServer.SendToAll(new DiceMissionStartedMsg() { });
    }

    private void PlayerClickedDraw(NetworkConnection conn)
    {
        if (!GameInfo.Players.TryGetValue(conn, out Player ply)) return;
        //If the player isn't on the mission
        if (!Players.Contains(ply)) return;
        CardInfo.TryGetValue(ply, out Deck deck);

        Card handCard = deck.Hand[0];
        deck.Discard(handCard);

        deck.Draw();
    }

    private void PlayerClickedSubmit(NetworkConnection conn)
    {
        if (!GameInfo.Players.TryGetValue(conn, out Player ply)) return;
        //If the player isn't on the mission
        if (!Players.Contains(ply)) return;
        CardInfo.TryGetValue(ply, out Deck deck);

        Card card = deck.Play();

        playedTotal += card.Value;

        if (!playersPlayed.Contains(ply))
        {
            playersPlayed.Add(ply);
            if (playersPlayed.Count >= Players.Count)
            {
                AllPlayersPlayed();
            }
        }
    }

    private void AllPlayersPlayed()
    {
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
    }
}

public struct DrawCardMsg : NetworkMessage
{
    public Card drawnCard;
}

public struct PlayerPlayedMsg : NetworkMessage
{

}

public struct AllPlayersPlayedMsg : NetworkMessage
{
    
}