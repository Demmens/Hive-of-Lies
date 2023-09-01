using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMPro.TMP_Text description;
    [SerializeField] GameObject cost;
    [SerializeField] TMPro.TMP_Text costText;

    public event System.Action<Card> OnClick;

    Card card;
    int quantity;

    private void Start()
    {
        SetCard(card);
    }

    public void SetCard(Card newCard)
    {
        if (newCard == null) return;
        card = newCard;
        image.sprite = card.Sprite;
        costText.text = newCard.BuyValue.ToString();
    }

    public Card GetCard()
    {
        return card;
    }

    public void ShowCost(bool active)
    {
        cost.gameObject.SetActive(active);
    }

    public void Pop()
    {
        image.transform.localScale = new Vector3(1.1f,1.1f,1.1f);
    }

    public void UnPop()
    {
        image.transform.localScale = new Vector3(1f,1f,1f);
    }

    public void Click()
    {
        OnClick?.Invoke(card);
    }
}
