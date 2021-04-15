using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct Point
{
    public readonly int x;
    public readonly int z;

    public Point(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public float GetDistance(Point p)
    {
        float xDiff = p.x - x;
        float zDiff = p.z - z;
        return Mathf.Sqrt(xDiff * xDiff + zDiff * zDiff);
    }

    public float GetManhattanDistance(Point p)
    {
        return Mathf.Abs(p.x - x) + Mathf.Abs(p.z - z);
    }

    public bool Same(Point p)
    {
        return p.x == x && p.z == z;
    }
}
