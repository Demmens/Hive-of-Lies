using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Mirror;

public class PredatorAbility : RoleAbility
{
    [SerializeField] GameObject targetPopup;
    [SerializeField] LocalizedString targetPopupText;
    public void OnTargetGiven()
    {
        RevealPredator(Owner.Target.Value.connectionToClient, Owner.DisplayName);
    }

    [TargetRpc]
    void RevealPredator(NetworkConnection conn, string predatorName)
    {
        targetPopup = Instantiate(targetPopup);
        targetPopup.GetComponent<Notification>().SetText(string.Format(targetPopupText.GetLocalizedString(), predatorName));
    }
}
