using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides functionalities for easing functions.
/// </summary>
public class Easing
{
    /// <summary>
    /// Function that provides an out-back ease
    /// </summary>
    /// <param name="elapsedTime"></param>
    /// <param name="begin"></param>
    /// <param name="change"></param>
    /// <param name="duration"></param>
    /// <param name="s"></param>
    /// <returns>result</returns>
    public static float OutBack(float elapsedTime, float begin, float change, float duration, float s = 1.70158f)
    {
        elapsedTime = elapsedTime / duration - 1;

        return change * (elapsedTime * elapsedTime * ((s + 1) * elapsedTime + s) + 1) + begin;
    }

    /// <summary>
    /// Function that provides an in-back ease
    /// </summary>
    /// <param name="elapsedTime"></param>
    /// <param name="begin"></param>
    /// <param name="change"></param>
    /// <param name="duration"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    public static float InBack(float elapsedTime, float begin, float change, float duration, float s = 1.70158f)
    {
        elapsedTime = elapsedTime / duration;
        return change * elapsedTime * elapsedTime * ((s + 1) * elapsedTime - s) + begin;
    }
}
