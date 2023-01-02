using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayAgainButton : NetworkBehaviour
{
    [Tooltip("Invoked when all players have pressed this button")]
    [SerializeField] GameEvent resetRound;

    [Tooltip("The number of players in the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("The game object containing the play again button")]
    [SerializeField] GameObject buttonObject;

    [Tooltip("The text displaying the winners")]
    [SerializeField] TMPro.TMP_Text winTextObject;

    [SyncVar(hook = nameof(SetTextObject))] string winText;

    List<NetworkConnection> playersClicked = new();

    [Tooltip("The list of players we are waiting on")]
    [SerializeField] TMPro.TMP_Text waitingFor;

    [Client]
    public void Click()
    {
        OnClientClicked();
        buttonObject.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    void OnClientClicked(NetworkConnectionToClient conn = null)
    {
        if (playersClicked.Contains(conn)) return;
        playersClicked.Add(conn);

        if (playersClicked.Count == playerCount) resetRound?.Invoke();
    }

    [Server]
    public void SetText(string text)
    {
        winText = text;
    }

    void SetTextObject(string oldText, string newText)
    {
        winTextObject.text = newText;
    }
}
