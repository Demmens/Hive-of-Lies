using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{

    #region Properties

    /// <summary>
    /// Amount of research needed for the Bees to win the game
    /// </summary>
    public int ResearchNeededForWin = 3;

    /// <summary>
    /// How much honey has to be stolen before the wasps win
    /// </summary>
    public int HoneyNeededForWin = 3;

    #endregion

    #region Events

    /// <summary>
    /// Delegate for <see cref="OnGameEnded"/>
    /// </summary>
    /// <param name="winningTeam">The team that won the game</param>
    public delegate void GameEnded(Team winningTeam);
    /// <summary>
    /// Invoked just before the game ends
    /// </summary>
    public event GameEnded OnGameEnded;

    #endregion

    /// <summary>
    /// Call to end the game
    /// </summary>
    /// <param name="WinningTeam">The team that won the game</param>
    public void EndGame(Team winningTeam)
    {
        OnGameEnded?.Invoke(winningTeam);
        Debug.Log($"{winningTeam}s won the game");
    }
}