using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPlanner : Singleton<PathPlanner>
{
    [SerializeField]
    private LineRenderer pathEstimate;

    private List<Point> path;

    public delegate void PathFoundEvent(List<Point> path);
    public PathFoundEvent OnPathFound;
    public PathFoundEvent OnPathNotFound;

    public void DrawPath(List<Point> path)
    {
        int len = path.Count;
        Vector3[] positions = new Vector3[len];
        for (int i = 0; i < len; i++)
        {
            Point point = path[i];
            positions[i] = new Vector3(point.x, 0, point.z);
        }
        pathEstimate.positionCount = len;
        pathEstimate.SetPositions(positions);
    }

    public IEnumerator GetPath(TerrainGrid grid, Point start, Point end)
    {
        //List<Point> open = new List<Point>();
        //List<Point> closed = new List<Point>();

        int openCount = 0;
        // 0 - unchecked
        // 1 - open
        // 2 - closed
        int[,] tileStatus;
        Heap<Point> scoreHeap = new Heap<Point>();

        Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
        Dictionary<Point, float> gScore = new Dictionary<Point, float>();
        Dictionary<Point, float> fScore = new Dictionary<Point, float>();

        float[,] values = grid.GetValues();
        tileStatus = new int[values.GetLength(0), values.GetLength(1)];

        gScore[start] = 0;
        fScore[start] = GetHScore(start, end);

        //open.Add(start);

        tileStatus[start.x, start.z] = 1;
        openCount++;
        scoreHeap.Add(start, fScore[start]);

        //Debug.Log("Path planning started");
        
        while (openCount > 0)
        {
            Point current = scoreHeap.Remove().GetItem();
            //Point current = GetLowestF(fScore, open);
            if (current.Same(end))
            {
                //Debug.Log("Path found");
                //yield return new WaitForSeconds(0.01f);
                List<Point> path = ReconstructPath(cameFrom, current);
                OnPathFound?.Invoke(path);
                yield break;
            }
            openCount--;
            tileStatus[current.x, current.z] = 2;
            //open.Remove(current);
            //closed.Add(current);

            List<Point> neighbors = grid.GetWalkableAdjacentPoints(current);

            //Debug.Log(string.Format("{0}, {1} ({2})", current.x, current.z, neighbors.Count));
            //yield return new WaitForSeconds(0.01f);
            //yield return null;

            foreach (Point neighbor in neighbors)
            {
                //if (!closed.Contains(neighbor))
                float tentativeGScore = GetGScore(gScore, current) + GetDistance(current, neighbor);
                if (tentativeGScore < GetGScore(gScore, neighbor))
                {
                    /*
                    if (!neighbor.Same(start))
                    {
                        
                    }
                    */
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    float tempFScore = tentativeGScore + GetHScore(neighbor, end);
                    fScore[neighbor] = tempFScore;
                    //if (!open.Contains(neighbor))
                    if (tileStatus[neighbor.x, neighbor.z] == 0)
                    {
                        tileStatus[neighbor.x, neighbor.z] = 1;
                        openCount++;
                        scoreHeap.Add(neighbor, tempFScore);
                        //open.Add(neighbor);
                    }
                }
            }
        }

        Debug.LogWarning("Path planner couldn't find a path!");
        OnPathNotFound?.Invoke(null);
        yield return null;
    }

    private static float GetHScore(Point p, Point end)
    {
        return p.GetManhattanDistance(end);
    }

    private static Point GetLowestF(Dictionary<Point, float> fScore, List<Point> open)
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

    private static float GetGScore(Dictionary<Point, float> gScore, Point p)
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

    private static float GetDistance(Point a, Point b)
    {
        return 1;
    }

    private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
    {
        List<Point> totalPath = new List<Point>();
        totalPath.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        totalPath.Reverse();
        return totalPath;
    }
}
