using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Class with helpful and often used list and math functions
/// </summary>
public static class Helper
{
    public static T GetRandomElement<T>(this List<T> lst)
    {
        return lst[UnityEngine.Random.Range(0, lst.Count - 1)];
    }

    public static int GetWeightedRandom(this List<float> weights)
    {
        List<float> cumulativeWeights = new List<float>();
        cumulativeWeights.Add(weights[0]);

        for (int i = 1; i < weights.Count; i++)
        {
            cumulativeWeights.Add(cumulativeWeights.Last() + weights[i]);
        }

        float val = UnityEngine.Random.Range(0, cumulativeWeights.Last());

        for (int i = 0; i < cumulativeWeights.Count; i++)
        {
            if (cumulativeWeights[i] <= val)
            {
                return i;
            }
        }
        return -1;
    }

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
