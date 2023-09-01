using UnityEngine;
using UnityEngine.Localization;
using Mirror;

public class KnowAllStingTargets : RoleAbility
{
    [SerializeField] HivePlayerSet waspPlayers;
    [SerializeField] GameObject popup;
    [SerializeField] LocalizedString popupText;
    [SerializeField] LocalizedString noTargetsText;

    public void AfterTargetsSelected()
    {
        string targetString = "";

        foreach (HivePlayer ply in waspPlayers.Value)
        {
            if (ply.Target.Value == null) continue;

            string playerName = ply.Target.Value.DisplayName;
            string roleName = ply.Target.Value.Role.Value.Data.RoleName;
            targetString += string.Format(popupText.GetLocalizedString(), playerName, roleName);
            targetString += "\n\n";
        }
        targetString.TrimEnd('\n');

        if (targetString == "") targetString = noTargetsText.GetLocalizedString();

        CreatePopup(targetString);
    }

    [TargetRpc]
    void CreatePopup(string text)
    {
        popup = Instantiate(popup);
        popup.GetComponent<Notification>().SetText(text);
    }
}
