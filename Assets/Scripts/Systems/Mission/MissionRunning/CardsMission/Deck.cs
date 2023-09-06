using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
    /// Invoked when a card is publicly removed from the draw pile of a player
    /// </summary>
    public event System.Action<Card> OnCardRemovedFromDrawPile;

    /// <summary>
    /// Invoked when a card is publicly added to the draw pile of a player
    /// </summary>
    public event System.Action<Card> OnCardAddedToDrawPile;

    /// <summary>
    /// Invoked when a card is publicly removed from the discard pile of a player
    /// </summary>
    public event System.Action<Card> OnCardRemovedFromDiscardPile;

    /// <summary>
    /// Invoked when a card is publicly added to the discard pile of a player
    /// </summary>
    public event System.Action<Card> OnCardAddedToDiscardPile;

    /// <summary>
    /// Invoked when a card is drawn
    /// </summary>
    public event DrawDelegate BeforeDraw;
    public delegate void DrawDelegate(ref Card card, bool simulated = false);

    /// <summary>
    /// Invoked when a card is played
    /// </summary>
    public event System.Action<Card> AfterPlay;

    /// <summary>
    /// Add a card to the deck
    /// </summary>
    public void Add(Card card)
    {
        DiscardPile.Add(card);
    }

    /// <summary>
    /// Removes a card from the draw pile. This will show as missing in that players deck screen.
    /// </summary>
    /// <param name="card">The card that was removed</param>
    public void PublicRemoveFromDraw(Card card)
    {
        if (!DrawPile.Contains(card)) return;

        DrawPile.Remove(card);
        OnCardRemovedFromDrawPile?.Invoke(card);
    }

    /// <summary>
    /// Adds a card to the deck. That card will appear in that players deck screen.
    /// </summary>
    /// <param name="card">The card that was added</param>
    public void PublicAddToDraw(Card card)
    {
        DrawPile.Add(card);
        OnCardAddedToDrawPile?.Invoke(card);
    }

    /// <summary>
    /// Removes a card from the discard pile. This will show as missing in that players deck screen.
    /// </summary>
    /// <param name="card">The card that was removed</param>
    public void PublicRemoveFromDiscard(Card card)
    {
        if (!DiscardPile.Contains(card)) return;

        DiscardPile.Remove(card);
        OnCardRemovedFromDiscardPile?.Invoke(card);
    }

    /// <summary>
    /// Adds a card to the discard pile. That card will appear in that players deck screen.
    /// </summary>
    /// <param name="card">The card that was added</param>
    public void PublicAddToDiscard(Card card)
    {
        DiscardPile.Add(card);
        OnCardAddedToDiscardPile?.Invoke(card);
    }

    /// <summary>
    /// Adds a card to the deck. That card will appear in that players deck screen.
    /// </summary>
    /// <param name="card">The card that was added</param>
    public void PublicAddToDeck(Card card)
    {
        DrawPile.Add(card);
        OnCardAddedToDrawPile?.Invoke(card);
    }

    /// <summary>
    /// Shuffle your discard pile into the deck
    /// </summary>
    public void Shuffle()
    {
        for (int i = DiscardPile.Count-1; i >= 0; i--)
        {
            Card card = DiscardPile[i];
            if (card.IsSecret) DiscardPile.Remove(card);
            else PublicRemoveFromDiscard(card);

            if (card.DestroyOnDraw) continue;

            if (card.IsSecret) DrawPile.Add(card);
            else PublicAddToDeck(card);
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
            PublicAddToDeck(card);
        }
        Hand = new();

        foreach (Card card in Played)
        {
            if (card.DestroyOnDraw || card.DestroyOnPlay) continue;
            PublicAddToDeck(card);
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

            card.DrawEffects.ForEach(effect => effect.TriggerEffect());

            BeforeDraw?.Invoke(ref card);

            Hand.Add(card);

            //It's no longer secret if the player has drawn and seen it.
            card.IsSecret = false;

            if (DrawPile.Contains(card)) PublicRemoveFromDraw(card);

            OnDraw?.Invoke(card);
        }
    }

    /// <summary>
    /// Returns the top card of the players deck, accounting for role manipulation
    /// </summary>
    public Card GetTopCard()
    {
        if (DrawPile.Count == 0)
        {
            if (DiscardPile.Count == 0)
            {
                Debug.LogError("Tried to draw from a deck with no cards");
            }
            Shuffle();
        }

        Card card = DrawPile[0];
        BeforeDraw?.Invoke(ref card, simulated:true);

        return card;
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

        card.DiscardEffects.ForEach(effect => effect.TriggerEffect());
        PublicAddToDiscard(card);
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
        card.PlayEffects.ForEach(effect => effect.TriggerEffect());
        Hand.Remove(card);
        AfterPlay?.Invoke(card);
        return card;
    }

    public Deck()
    {

    }
}
