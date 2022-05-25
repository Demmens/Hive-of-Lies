using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    /// <summary>
    /// The player that owns this deck
    /// </summary>
    public Player Owner;
    /// <summary>
    /// The cards in the draw pile
    /// </summary>
    public List<Card> DrawPile = new List<Card>();

    /// <summary>
    /// The cards in the discard pile
    /// </summary>
    public List<Card> DiscardPile = new List<Card>();

    /// <summary>
    /// The cards in a players hand
    /// </summary>
    public List<Card> Hand = new List<Card>();

    /// <summary>
    /// The cards the player has played
    /// </summary>
    public List<Card> Played = new List<Card>();

    /// <summary>
    /// Add a card to the deck
    /// </summary>
    public void Add(Card card)
    {
        DiscardPile.Add(card);
    }

    /// <summary>
    /// Shuffle your discard pile into the deck
    /// </summary>
    public void Shuffle()
    {
        DrawPile.AddRange(DiscardPile);
        DrawPile.Shuffle();
    }

    /// <summary>
    /// Shuffle your discard pile, hand, and played cards into the deck
    /// </summary>
    public void Reshuffle()
    {
        DrawPile.AddRange(Hand);
        DrawPile.AddRange(Played);
        Shuffle();
    }

    /// <summary>
    /// Draw cards from the deck
    /// </summary>
    public void Draw(int cards = 1)
    {
        for (int i = 0; i < cards; i++)
        {
            if (DrawPile.Count == 0)
            {
                if (DiscardPile.Count == 0) return;
                Shuffle();
            }

            if (DrawPile.Count == 0)
            {
                Debug.LogError("Tried to draw from a deck with no cards.");
                return;
            }

            DrawPile[0].DrawEffects.ForEach(effect => effect());

            Hand.Add(DrawPile[0]);
            DrawPile.RemoveAt(0);
        }
    }

    /// <summary>
    /// Discard a card from the hand
    /// </summary>
    public void Discard(Card card = null)
    {
        if (Hand.Count == 0) return;
        // By default discard the oldest card in the hand
        if (card == null) card = Hand[0];
        if (!Hand.Contains(card)) return;

        card.DiscardEffects.ForEach(effect => effect());

        DiscardPile.Add(card);
        Hand.Remove(card);
    }

    /// <summary>
    /// Play a card from the hand
    /// </summary>
    public Card Play(Card card = null)
    {
        if (Hand.Count == 0) return null;
        // By default play the oldest card in the hand
        if (card == null) card = Hand[0];
        if (!Hand.Contains(card)) return null;

        Played.Add(card);
        card.PlayEffects.ForEach(effect => effect());
        Hand.Remove(card);
        return card;
    }

    public Deck(List<Card> starting)
    {
        DrawPile = starting;
    }

    public Deck(List<int> starting)
    {
        for (int i = 0; i < starting.Count; i++)
        {
            DrawPile.Add(new Card(starting[i]));
        }
    }
}
