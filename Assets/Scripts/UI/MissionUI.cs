using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;

public class MissionUI : MonoBehaviour
{
    [SerializeField] GameObject missionUI;
    [SerializeField] TMP_Text missionName;
    [SerializeField] TMP_Text missionFlavour;
    [SerializeField] TMP_Text successFlavour;
    [SerializeField] TMP_Text successEffect;
    [SerializeField] TMP_Text failFlavour;
    [SerializeField] TMP_Text failEffect;
    [SerializeField] GameObject missionCard;
    [SerializeField] Transform OverlayCanvas;

    [SerializeField] TMP_Text teamLeaderName;
    [SerializeField] TMP_Text missionPlayerList;
    [SerializeField] TMP_Text missionCost;

    List<GameObject> cards = new List<GameObject>();

    private void Start()
    {
        // Listen for server sending us information
        NetworkClient.RegisterHandler<SendMissionChoicesMsg>(CreateMissionCards);
        NetworkClient.RegisterHandler<SendDecidedMissionMsg>(ChangeMission);
        NetworkClient.RegisterHandler<TeamLeaderChangePartnersMsg>(OnTeamLeaderChangePartner);

        ClientEventProvider.singleton.OnTeamLeaderChanged += OnTeamLeaderDecided;
    }

    /// <summary>
    /// Creates the mission cards on the screen
    /// </summary>
    /// <param name="choices"></param>
    void CreateMissionCards(SendMissionChoicesMsg msg)
    {
        Debug.Log("Received mission choices");
        for (int i = 0; i < msg.choices.Count; i++)
        {
            GameObject card = Instantiate(missionCard, GetCardPositionOnScreen(i, msg.choices.Count), new Quaternion());
            card.transform.SetParent(OverlayCanvas);

            MissionCard cardScript = card.GetComponent<MissionCard>();
            cardScript.SetData(msg.choices[i]);
            cardScript.OnMissionCardClicked += MissionCardClicked;
            cards.Add(card);
            
        }
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

    /// <summary>
    /// Creates a string from a list of mission effects
    /// </summary>
    /// <param name="list">The list of mission effects</param>
    /// <returns></returns>
    public static string CreateStringFromList(List<MissionEffect> list)
    {
        string res = "";
        for (int i = 0; i < list.Count; i++)
        {
            res += list[i].Description;
            if (i != list.Count - 1)
            {
                res += ", ";
            }
        }
        if (res == "") res = "No effect";
        return res;
    }

    void MissionCardClicked(MissionData data)
    {
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }

        NetworkClient.Send(new PlayerVotedOnMissionMsg()
        {
            mission = data
        });
    }

    void ChangeMission(SendDecidedMissionMsg msg)
    {
        cards = new List<GameObject>();
        missionUI.SetActive(true);
        missionName.text = msg.mission.MissionName;
        missionFlavour.text = msg.mission.Description;
        successFlavour.text = msg.mission.SuccessFlavour;
        failFlavour.text = msg.mission.FailFlavour;
        missionCost.text = $"Mission Cost: {msg.mission.FavourCost}f";

        successEffect.text = CreateStringFromList(msg.mission.SuccessEffects);
        failEffect.text = CreateStringFromList(msg.mission.FailEffects);
    }

    /// <summary>
    /// Called when the team leader is decided
    /// </summary>
    /// <param name="msg"></param>
    void OnTeamLeaderDecided(TeamLeaderChangedMsg msg)
    {
        teamLeaderName.text = SteamFriends.GetFriendPersonaName(msg.ID);
    }

    /// <summary>
    /// Called when the team leader has added or removed a player from the mission.
    /// </summary>
    /// <param name="msg"></param>
    void OnTeamLeaderChangePartner(TeamLeaderChangePartnersMsg msg)
    {
        Debug.Log("Team Leader has changed partners");
        string playerName = SteamFriends.GetFriendPersonaName(msg.playerID);
        if (msg.selected)
        {
            //If we haven't selected any players yet, clear the text, otherwise add a line break for the next player
            missionPlayerList.text = (missionPlayerList.text == "Undecided") ? "" : missionPlayerList.text + "\n";

            missionPlayerList.text += playerName;
        }
        else
        {
            missionPlayerList.text.Replace(playerName, "");
            //Make sure to remove duplicate line breaks
            missionPlayerList.text.Replace("\n\n", "\n");
        }
    }
}