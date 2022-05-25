using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class MissionResultPopup : MonoBehaviour
{
    [SerializeField] GameObject popup;
    [SerializeField] TMP_Text missionResult;
    [SerializeField] TMP_Text outcomeFlavour;
    [SerializeField] TMP_Text rollResults;

    void Start()
    {
        NetworkClient.RegisterHandler<CreateMissionResultPopupMsg>(OnMissionEnded);
    }

    void OnMissionEnded(CreateMissionResultPopupMsg msg)
    {
        string rollResultsText = "";
        bool wasOnMission = ClientGameInfo.CurrentlySelected.Contains(ClientGameInfo.PlayerID);

        for (int i = 0; i < ClientGameInfo.CurrentlySelected.Count; i++)
        {
            if (i != 0) rollResultsText += ", ";
            rollResultsText += wasOnMission ? msg.finalCards[i].ToString() : "??";
        }
        rollResults.text = rollResultsText;

        bool success = msg.result == MissionResult.Success;

        missionResult.text = success ? "Mission Succeeded" : "Mission Failed";

        outcomeFlavour.text = success ? "[Success Flavour]" : "[Fail Flavour]";

        popup.SetActive(true);
    }

    public void OnClose()
    {
        popup.SetActive(false);
        NetworkClient.Send(new ClosedMissionResultPopupMsg() { });
    }
}

public struct ClosedMissionResultPopupMsg : NetworkMessage
{

}
