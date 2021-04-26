using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static float Clamp(this float f, float min, float max)
    {
        return Mathf.Min(max, Mathf.Max(min, f));
    }
}
