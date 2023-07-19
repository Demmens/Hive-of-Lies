using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMPro.TMP_Text description;

    Card card;
    int quantity;

    private void Start()
    {
        SetCard(card);
    }

    public void SetCard(Card newCard)
    {
        if (card == null) return;
        card = newCard;
        image.sprite = card.Sprite;
    }

    public Card GetCard()
    {
        return card;
    }

    public void Pop()
    {
        image.transform.localScale = new Vector3(1.1f,1.1f,1.1f);
    }

    public void UnPop()
    {
        image.transform.localScale = new Vector3(1f,1f,1f);
    }
}
