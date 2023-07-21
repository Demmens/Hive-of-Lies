using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Coroutines
{
    /// <summary>
    /// Runs a function after a delay
    /// </summary>
    /// <param name="seconds">How long the delay should be</param>
    /// <param name="func">The function to run</param>
    /// <returns></returns>
    public static IEnumerator Delay(float seconds, System.Action func)
    {
        yield return new WaitForSeconds(seconds);
        func();
    }

    /// <summary>
    /// Runs a function the next frame
    /// </summary>
    /// <param name="func">The function to run</param>
    /// <returns></returns>
    public static IEnumerator Delay(System.Action func)
    {
        yield return null;
        func();
    }

    /// <summary>
    /// Runs many functions after specified delays
    /// </summary>
    /// <param name="funcs">Tuple. First item is the delay, second item is the function to run after the delay.</param>
    /// <returns></returns>
    public static IEnumerator Delay((float,System.Action)[] funcs)
    {
        for (int i = 0; i < funcs.Length; i++)
        {
            yield return new WaitForSeconds(funcs[i].Item1);
            funcs[i].Item2();
        }
        
    }
}
