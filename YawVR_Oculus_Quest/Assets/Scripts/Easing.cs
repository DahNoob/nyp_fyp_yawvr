using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Easing
{
    public static float OutBack(float elapsedTime, float begin, float change, float duration, float s = 1.70158f)
    {
        elapsedTime = elapsedTime / duration - 1;

        return change * (elapsedTime * elapsedTime * ((s + 1) * elapsedTime + s) + 1) + begin;
    }

    public static float InBack(float elapsedTime, float begin, float change, float duration, float s = 1.70158f)
    {
        elapsedTime = elapsedTime / duration;
        return change * elapsedTime * elapsedTime * ((s + 1) * elapsedTime - s) + begin;
    }
}
