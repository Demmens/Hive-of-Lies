using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PhaseController : MonoBehaviour
{
    /// <summary>
    /// Reference to the setup game phase.
    /// </summary>
    [SerializeField] Setup setup;

    /// <summary>
    /// The game object that contains all the phases.
    /// </summary>
    [SerializeField] List<GamePhase> phases;

    [SerializeField] int favourGainPerRound;

    /// <summary>
    /// The index of the current phase of the game.
    /// </summary>
    int currentPhase;

    [Tooltip("The round number")]
    [SerializeField] IntVariable roundNum;

    [Tooltip("All players")]
    [SerializeField] HoLPlayerSet players;

    [Tooltip("Reset the round when this event is invoked")]
    [SerializeField] GameEvent resetRound;

    void Start()
    {
        //Give all events a reference to the event system. Saves having to do a FindObjectOfType on each child class of GamePhase.
        foreach (GamePhase phase in phases)
        {
            //Listen for when phases end
            phase.OnGamePhaseEnd += PhaseChange;
        }
        //Make sure to listen for the setup ending too.
        setup.OnGamePhaseEnd += PhaseChange;
        //Set to -1 since on a phase end it increments, and we want to start at phases[0].
        currentPhase = -1;
        //Complete the setup first.
        setup.ChangePhase();
    }

    /// <summary>
    /// Called when the current game phase ends
    /// </summary>
    void PhaseChange()
    {
        //Move to the next phase
        currentPhase++;
        //Make sure to loop back to the beginning again once we reach the last phase
        if (currentPhase >= phases.Count)
        {
            StartNextRound();
        }
        else
        {
            //Begin the next phase
            phases[currentPhase].ChangePhase();
        }
    }

    /// <summary>
    /// Call to reset the round back to the beginning.
    /// </summary>
    public void ResetRound()
    {
        phases[currentPhase].End(true);

        StartNextRound();
    }

    void StartNextRound()
    {
        currentPhase = 0;
        roundNum++;

        foreach (HoLPlayer ply in players.Value)
        {
            ply.Favour.Value += favourGainPerRound;
        }

        NetworkServer.SendToAll(new ChangeFavourMsg()
        {
            favourIncrease = favourGainPerRound
        });

        phases[0].ChangePhase();
    }
}
