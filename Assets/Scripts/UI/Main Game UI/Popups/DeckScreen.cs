using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeckScreen : NetworkBehaviour
{
    [SerializeField] HoLPlayerDictionary playersByConnection;

    #region Client
    [SerializeField] GameObject cardDisplay;
    [SerializeField] Transform cardPool;

    List<CardDisplay> drawPile = new();
    List<CardDisplay> discardPile = new();
    #endregion

    private void Start()
    {
        foreach (KeyValuePair<NetworkConnection,HoLPlayer> pair in playersByConnection.Value)
        {
            pair.Value.Deck.Value.OnCardRemovedFromDrawPile += (card) => CardRemoved(pair.Key, card);
            pair.Value.Deck.Value.OnCardAddedToDrawPile += (card) => CardAdded(pair.Key, card);
        }
    }

    [TargetRpc]
    void CardRemoved(NetworkConnection conn, Card card)
    {
        CardDisplay display = null;

        foreach (CardDisplay c in drawPile)
        {
            if (c.GetCard() == card) display = c;
        }

        //If that card never showed up in their draw pile in the first place, we don't need to do anything else.
        if (display == null) return;

        //Otherwise destroy it.
        drawPile.Remove(display);
        Destroy(display);
    }

    [TargetRpc]
    void CardAdded(NetworkConnection conn, Card card)
    {
        CardDisplay display = Instantiate(cardDisplay).GetComponent<CardDisplay>();
        display.SetCard(card);
        display.transform.SetParent(cardPool);
    }
}
