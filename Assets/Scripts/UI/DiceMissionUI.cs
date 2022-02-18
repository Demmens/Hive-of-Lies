using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceMissionUI : MonoBehaviour
{
    [SerializeField] TMP_Text RollResult;
    [SerializeField] TMP_Text RollNeeded;
    [SerializeField] GameObject RollButton;

    private void Start()
    {
        // Listen for server sending us information
    }

    public void RerollDice()
    {
        RollResult.text = "-";
        // Send empty to server
    }

    public void LockInDice()
    {
        // Send empty to server
    }

    void OnInfluenceLoss()
    {
        // If influence is less than the cost of the next reroll, disable the reroll button.
    }

    void ReceiveRollFromServer()
    {
        //RollResult.text = "";
    }
}
