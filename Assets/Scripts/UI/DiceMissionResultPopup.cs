using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class DiceMissionResultPopup : MonoBehaviour
{
    [SerializeField] GameObject popup;
    [SerializeField] TMP_Text resultText;

    void Start()
    {
        NetworkClient.RegisterHandler<MissionEndMsg>(OnMissionEnded);
    }
    void OnMissionEnded(MissionEndMsg msg)
    {
        switch (msg.result)
        {
            case MissionResult.Success:
                resultText.text = "The mission was successful!";
                break;
            case MissionResult.Fail:
                resultText.text = "The mission was a failure";
                break;
        }
        popup.SetActive(true);
    }
}
