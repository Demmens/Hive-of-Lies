using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class RoleUI : MonoBehaviour
{
    List<GameObject> cards;

    [SerializeField] TMP_Text RoleName;
    [SerializeField] FavourController Favour;
    [SerializeField] GameObject RoleCard;
    [SerializeField] Transform OverlayCanvas;
    [SerializeField] Image RoleBackground;


    [SerializeField] Color WaspColour;
    [SerializeField] Color BeeColour;
 
    private void Start()
    {
        NetworkClient.RegisterHandler<SendRoleInfoMsg>(ReceiveRoleInfo);
        cards = new List<GameObject>();
    }

    void ReceiveRoleInfo(SendRoleInfoMsg msg)
    {
        
        for (int i = 0; i < msg.roleChoices.Count; i++)
        {
            GameObject card = Instantiate(RoleCard, GetCardPositionOnScreen(i, msg.roleChoices.Count), new Quaternion());
            card.transform.SetParent(OverlayCanvas);
            RoleCard cardScript = card.GetComponent<RoleCard>();
            cardScript.SetData(msg.roleChoices[i]);
            cardScript.OnRoleCardClicked += RoleCardClicked;
            cards.Add(card);
        }
    }

    void RoleCardClicked(Role data)
    {
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }

        RoleName.text = data.RoleName;
        Favour.Favour = data.StartingFavour;

        NetworkClient.Send(new PlayerSelectedRoleMsg()
        {
            role = data
        });
    }

    Vector3 GetCardPositionOnScreen(int index, int cardsTotal)
    {
        const float margin = 600;

        float adjustedWidth = Screen.width - (2 * margin);

        float x = Screen.width / 2;
        if (cardsTotal > 1)
        {
            x = margin + adjustedWidth * (index / (float)(cardsTotal - 1));
        }

        return new Vector3(x, Screen.height / 2, 0);
    }
}
