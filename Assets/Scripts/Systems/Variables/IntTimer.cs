using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Int Timer", menuName = "Variable/Timer/Int")]
public class IntTimer : IntVariable
{
    [Tooltip("How much the timer reduces by each tick")]
    [SerializeField] int step = 1;

    [Tooltip("How much time to wait between each tick")]
    [SerializeField] float tickLengthSeconds = 1;

    public event System.Action OnTimerEnd;

    bool stop;

    /// <summary>
    /// Time elapsed between current step and next step
    /// </summary>
    float elapsed;

    /// <summary>
    /// Starts the timer. Must be used as a parameter in a StartCoroutine call
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartTimer()
    {
        while (Value > 0)
        {
            if (stop)
            {
                stop = false;
                yield break;
            }

            yield return null;
            elapsed += Time.deltaTime;
            if (elapsed < tickLengthSeconds) continue;

            elapsed -= tickLengthSeconds;
            
            Value -= step;
        }

        OnTimerEnd?.Invoke();
    }

    public void StopTimer()
    {
        stop = true;
    }

    public void ResetTimer()
    {
        StopTimer();
        Value = initialValue;
        elapsed = 0;
    }
}
