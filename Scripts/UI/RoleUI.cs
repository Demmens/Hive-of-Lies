using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class RoleUI : MonoBehaviour
{
    List<GameObject> cards;

    [SerializeField] TMP_Text RoleName;
    [SerializeField] TMP_Text RoleDescription;
    [SerializeField] TMP_Text Team;
    [SerializeField] TMP_Text Favour;
    [SerializeField] GameObject RoleCard;
 
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
            RoleCard cardScript = card.GetComponent<RoleCard>();
            cardScript.SetData(msg.roleChoices[i]);
            cardScript.OnRoleCardClicked += RoleCardClicked;
            cards.Add(card);
            
        }
    }

    void RoleCardClicked(RoleData data)
    {
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }

        RoleName.text = data.RoleName;
        RoleDescription.text = data.Description;
        Team.text = data.Team.ToString();
        Favour.text = $"{data.StartingFavour}f";

        NetworkClient.Send(new PlayerSelectedRoleMsg()
        {
            role = data
        });
    }

    Vector3 GetCardPositionOnScreen(int index, int cardsTotal)
    {
        const int margin = 50;

        int x = (Screen.width - 2 * margin) * ((index + 1) / cardsTotal);

        return new Vector3(x, Screen.height / 2, 0);
    }
}
