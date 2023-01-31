using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    /// <summary>
    /// The cards in the draw pile
    /// </summary>
    public List<Card> DrawPile = new();

    /// <summary>
    /// The cards in the discard pile
    /// </summary>
    public List<Card> DiscardPile = new();

    /// <summary>
    /// The cards in a players hand
    /// </summary>
    public List<Card> Hand = new();

    /// <summary>
    /// The cards the player has played
    /// </summary>
    public List<Card> Played = new();

    /// <summary>
    /// Invoked when a card is drawn
    /// </summary>
    public event System.Action<Card> OnDraw;

    /// <summary>
    /// Invoked when a card is drawn
    /// </summary>
    public event DrawDelegate BeforeDraw;
    public delegate void DrawDelegate(ref Card card);

    /// <summary>
    /// Invoked when a card is played
    /// </summary>
    public event System.Action<Card> AfterPlay;

    /// <summary>
    /// Invoked when a card in the hand changes its value
    /// </summary>
    public event System.Action<int> HandCardValueChanged;

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
        foreach (Card card in DiscardPile)
        {
            if (card.DestroyOnDraw) continue;
            DrawPile.Add(card);
        }
        DrawPile.Shuffle();
    }

    /// <summary>
    /// Shuffle your discard pile, hand, and played cards into the deck
    /// </summary>
    public void Reshuffle()
    {
        foreach (Card card in Hand)
        {
            if (card.DestroyOnDraw) continue;
            card.TempValue = card.Value;
            DrawPile.Add(card);
        }
        Hand = new();

        foreach (Card card in Played)
        {
            if (card.DestroyOnDraw || card.DestroyOnPlay) continue;
            card.TempValue = card.Value;
            DrawPile.Add(card);
        }
        Played = new();
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
                if (DiscardPile.Count == 0)
                {
                    Debug.LogError("Tried to draw from a deck with no cards");
                    return;
                }
                Shuffle();
            }

            if (DrawPile.Count == 0)
            {
                Debug.LogError("Tried to draw from a deck with no cards.");
                return;
            }

            Card card = DrawPile[0];

            card.DrawEffects.ForEach(effect => effect());

            BeforeDraw?.Invoke(ref card);

            Hand.Add(card);

            card.OnValueChanged += (val) => HandCardValueChanged?.Invoke(val);

            if (DrawPile.Contains(card)) DrawPile.Remove(card);

            OnDraw?.Invoke(card);
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
        card.TempValue = card.Value;
        card.OnValueChanged -= (val) => HandCardValueChanged?.Invoke(val);
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
        AfterPlay?.Invoke(card);
        return card;
    }

    public Deck()
    {

    }
}
