using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPlanner : MonoBehaviour
{
    [SerializeField]
    private NoisyGrid grid;

    public List<Point> GetPath(Point start, Point end)
    {
        List<Point> open = new List<Point>();
        List<Point> closed = new List<Point>();

        Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
        Dictionary<Point, float> gScore = new Dictionary<Point, float>();
        Dictionary<Point, float> fScore = new Dictionary<Point, float>();

        gScore[start] = 0;
        fScore[start] = GetHScore(start, end);

        open.Add(start);

        while (open.Count > 0)
        {
            Point current = GetLowestF(fScore, open);
            if (current.Same(end))
            {
                return ReconstructPath(cameFrom, current);
            }

            open.Remove(current);

            foreach (Point neighbor in grid.GetWalkableAdjacentPoints(current))
            {
                float tentativeGScore = GetGScore(gScore, current) + GetDistance(current, neighbor);
                if (tentativeGScore < GetGScore(gScore, neighbor))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + GetHScore(neighbor, end);
                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                    }
                }
            }
        }

        Debug.LogWarning("Path planner couldn't find a path!");
        return null;
    }

    private float GetHScore(Point p, Point end)
    {
        return p.GetManhattanDistance(end);
    }

    private Point GetLowestF(Dictionary<Point, float> fScore, List<Point> open)
    {
        Point best = default;
        float score = Mathf.Infinity;

        foreach (Point p in open)
        {
            if (fScore.TryGetValue(p, out float value))
            {
                if (value < score)
                {
                    best = p;
                    score = value;
                }
            }
        }

        return best;
    }

    private float GetGScore(Dictionary<Point, float> gScore, Point p)
    {
        if (gScore.TryGetValue(p, out float value))
        {
            return value;
        }
        else
        {
            return Mathf.Infinity;
        }
    }

    private float GetDistance(Point a, Point b)
    {
        return 1;
    }

    private List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
    {
        List<Point> totalPath = new List<Point>();
        totalPath.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            totalPath.Add(cameFrom[current]);
        }
        totalPath.Reverse();
        return totalPath;
    }
}
