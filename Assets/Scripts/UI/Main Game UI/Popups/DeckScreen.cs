using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeckScreen : NetworkBehaviour
{
    [SerializeField] GameObject cardDisplay;
    [SerializeField] Transform cardPool;
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Client]
    public void DeckClicked()
    {
        RequestPopulation();
    }

    [Command(requiresAuthority = false)]
    public void RequestPopulation(NetworkConnectionToClient conn = null)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        List<Card> cardList = new();

        foreach (Card card in ply.Deck.Value.DrawPile)
        {
            if (card.IsSecret) continue;
            cardList.Add(card);
        }

        //Make sure to sort this list of cards, since the player shouldn't know what the order of their deck is.
        cardList.Sort((a, b) => { return a.Value - b.Value; });

        Populate(ply.connectionToClient, cardList);
    }

    [TargetRpc]
    public void Populate(NetworkConnection conn, List<Card> deck)
    {
        //Destroy all current cards in the pool. This isn't the best way of doing this, but it's the quickest to implement for now.
        //for (int i = cardPool.childCount-1; i >= 0; i--) Destroy(cardPool.GetChild(i));

        //Generate new ones
        foreach (Card card in deck)
        {
            CardDisplay display = Instantiate(cardDisplay).GetComponent<CardDisplay>();
            display.SetCard(card);
            display.transform.SetParent(cardPool);
        }
    }
}
