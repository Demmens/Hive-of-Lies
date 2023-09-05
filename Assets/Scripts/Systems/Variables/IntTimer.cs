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
    public bool IsRunning { get; private set; }

    bool stop;

    public IEnumerator StartTimer()
    {
        if (IsRunning)
        {
            Debug.LogError("Timer is already running");
            yield break;
        }
        IsRunning = true;

        while (Value > 0)
        {
            if (stop)
            {
                stop = false;
                IsRunning = false;
                yield break;
            }
            Value -= step;
            yield return new WaitForSeconds(tickLengthSeconds);
        }

        OnTimerEnd?.Invoke();
    }

    public void StopTimer()
    {
        stop = true;
    }
}
