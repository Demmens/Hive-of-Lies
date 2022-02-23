using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    /// <summary>
    /// Reference to the setup game phase.
    /// </summary>
    [SerializeField] Setup setup;

    /// <summary>
    /// The game object that contains all the phases.
    /// </summary>
    [SerializeField] GameObject phasesObject;

    /// <summary>
    /// Array of all the game phases we want to repeat for the gameplay loop.
    /// </summary>
    GamePhase[] phases;

    /// <summary>
    /// The index of the current phase of the game.
    /// </summary>
    int currentPhase;

    void Start()
    {
        phases = phasesObject.GetComponents<GamePhase>();
        //Give all events a reference to the event system. Saves having to do a FindObjectOfType on each child class of GamePhase.
        foreach (GamePhase phase in phases)
        {
            //Listen for when phases end
            phase.OnGamePhaseChange += new GamePhase.GamePhaseChange(PhaseChange);
            if (phase is StandOrPass)
            {
                StandOrPass sop = phase as StandOrPass;
                sop.OnNobodyStood += new StandOrPass.NobodyStood(ResetRound);
            }
        }
        //Make sure to listen for the setup ending too.
        setup.OnGamePhaseChange += new GamePhase.GamePhaseChange(PhaseChange);
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
        if (currentPhase >= phases.Length)
        {
            currentPhase = 0;
            GameInfo.RoundNum++;
        }
        //And begin the next phase
        phases[currentPhase].ChangePhase();
    }

    /// <summary>
    /// Call to reset the round back to the beginning.
    /// </summary>
    void ResetRound()
    {
        phases[currentPhase].End(true);
        currentPhase = 0;
        GameInfo.RoundNum++;
        phases[0].ChangePhase();
    }
}
