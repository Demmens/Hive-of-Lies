using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MissionUI : MonoBehaviour
{
    [SerializeField] TMP_Text missionName;
    [SerializeField] TMP_Text missionFlavour;
    [SerializeField] TMP_Text successFlavour;
    [SerializeField] TMP_Text successEffect;
    [SerializeField] TMP_Text failFlavour;
    [SerializeField] TMP_Text failEffect;

    List<MissionCard> cards;

    private void Start()
    {
        NetworkClient.RegisterHandler<SendMissionChoices>(CreateMissionCards);
        // Listen for server sending us information
    }

    public void ChangeMission(Mission mission)
    {
        missionName.text = mission.Data.Name;
        missionFlavour.text = mission.Data.Description;
        successFlavour.text = mission.Data.SuccessFlavour;
        failFlavour.text = mission.Data.FailFlavour;

        successEffect.text = CreateStringFromList(mission.SuccessEffects);
        failEffect.text = CreateStringFromList(mission.FailEffects);
    }

    /// <summary>
    /// Creates a string from a list of mission effects
    /// </summary>
    /// <param name="list">The list of mission effects</param>
    /// <returns></returns>
    public static string CreateStringFromList(List<MissionEffect> list)
    {
        string res = "";
        for (int i = 0; i <list.Count; i++)
        {
            res += list[i].Description;
            if (i != list.Count - 1)
            {
                res += ", ";
            }
        }
        return res;
    }

    /// <summary>
    /// Creates the mission cards on the screen
    /// </summary>
    /// <param name="choices"></param>
    void CreateMissionCards(SendMissionChoices msg)
    {
        for (int i = 0; i < msg.choices.Count; i++)
        {
            MissionCard card = new MissionCard()
            {
                Data = msg.choices[i]
            };
            card.OnMissionCardClicked += MissionCardClicked;
            cards.Add(card);
            Instantiate(card, GetCardPositionOnScreen(i, msg.choices.Count), new Quaternion());
        }
    }

    Vector3 GetCardPositionOnScreen(int index, int cardsTotal)
    {
        const int margin = 50;

        int x = (Screen.width - 2 * margin)*(index/cardsTotal);

        return new Vector3(x, Screen.height / 2, 0);
    }

    void MissionCardClicked(MissionData data)
    {
        foreach (MissionCard card in cards)
        {
            Destroy(card.gameObject);
        }

        NetworkClient.Send(new PlayerVotedOnMissionMsg()
        {
            mission = data
        });
    }
}