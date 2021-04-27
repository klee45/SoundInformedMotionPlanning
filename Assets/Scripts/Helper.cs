using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Helper
{
    public static float Clamp(this float f, float min, float max)
    {
        return Mathf.Min(max, Mathf.Max(min, f));
    }

    public static int Clamp(this int i, int min, int max)
    {
        return Math.Min(max, System.Math.Max(min, i));
    }

    public static float ToRadians(float degree)
    {
        return degree * Mathf.PI / 180f;
    }

    public static float ToDegrees(float radian)
    {
        return 180f * radian / Mathf.PI;
    }

    public static double ToRadians(double degree)
    {
        return degree * Math.PI / 180.0;
    }

    public static double ToDegrees(double radian)
    {
        return 180.0 * radian / Math.PI;
    }
}
