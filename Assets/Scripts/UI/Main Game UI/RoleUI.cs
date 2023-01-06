using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class RoleUI : NetworkBehaviour
{
    #region CLIENT
    List<GameObject> cards;

    [SerializeField] TMP_Text RoleName;
    [SerializeField] GameObject RoleCard;
    [SerializeField] Color WaspColour;
    [SerializeField] Color BeeColour;
    #endregion

    [Space]
    [Space]

    #region SERVER
    [Tooltip("All players by their network connection")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("Runtime set of all the roles in the game")]
    [SerializeField] RoleSet allRoles;

    [Tooltip("The amount of players in the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("The event to invoke when all players have chosen their roles")]
    [SerializeField] GameEvent allPlayersChosenRoles;
    #endregion

    public override void OnStartClient()
    {
        base.OnStartClient();
        cards = new List<GameObject>();
    }

    [Server]
    public void OnSetupFinished()
    {
        allRoles.Value = new();
        foreach (KeyValuePair<NetworkConnection, HoLPlayer> pair in playersByConnection.Value)
        {
            ReceiveRoleInfo(pair.Key, pair.Value.RoleChoices);
        }
    }

    [TargetRpc]
    public void ReceiveRoleInfo(NetworkConnection conn, List<RoleData> roleChoices)
    {
        for (int i = 0; i < roleChoices.Count; i++)
        {
            GameObject card = Instantiate(RoleCard);
            RoleCard cardScript = card.GetComponent<RoleCard>();
            cardScript.SetPos(GetCardPositionOnScreen(i, roleChoices.Count));
            cardScript.SetData(roleChoices[i]);
            cardScript.OnRoleCardClicked += RoleCardClicked;
            cards.Add(card);
        }
    }

    [Client]
    void RoleCardClicked(RoleData data)
    {
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }

        RoleName.text = data.RoleName;

        AssignPlayerRole(data);
    }

    [Command(requiresAuthority = false)]
    void AssignPlayerRole(RoleData data, NetworkConnectionToClient conn = null)
    {

        //If the player doesn't exist
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        //If the player already has a role
        if (ply.Role.Value != null) return;
        //If the role selected isn't one of the players choices
        if (!ply.RoleChoices.Contains(data)) return;

        GameObject abilityObject = Instantiate(data.Ability);
        RoleAbility ability = abilityObject.GetComponent<RoleAbility>();
        ability.Owner = ply;
        ability.OwnerConnection = conn;
        NetworkServer.Spawn(ability.gameObject, conn);
        ply.Favour.Value = data.StartingFavour;

        Role role = new()
        {
            Ability = ability,
            Data = data
        };

        allRoles.Add(role);
        ply.Role.Value = role;

        if (allRoles.Value.Count == playerCount) allPlayersChosenRoles?.Invoke();
    }

    static Vector3 GetCardPositionOnScreen(int index, int cardsTotal)
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
