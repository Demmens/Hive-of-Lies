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

    /// <summary>
    /// Starts the timer. Must be used as a parameter in a StartCoroutine call
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartTimer()
    {
        while (Value > 0)
        {
            yield return new WaitForSeconds(tickLengthSeconds);
            Value -= step;
        }

        OnTimerEnd?.Invoke();
    }

    public void ResetTimer()
    {
        Value = initialValue;
    }
}
