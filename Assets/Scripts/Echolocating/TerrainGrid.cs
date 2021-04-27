using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGrid : MonoBehaviour
{
    protected float[,] grid;

    public float GetCellValue(Point p)
    {
        return grid[p.x, p.z];
    }

    public void InitializeSize(int width, int height)
    {
        grid = new float[width, height];
    }

    public void SetCellValue(int x, int z, float val)
    {
        grid[x, z] = val;
    }

    public float[,] GetValues()
    {
        return grid;
    }

    public int GetWidth() { return grid.GetLength(0); }
    public int GetHeight() { return grid.GetLength(1); }

    public Point GetCloseFree(float x, float z)
    {
        int xClamp = Mathf.RoundToInt(x.Clamp(0, grid.GetLength(0) - 1));
        int zClamp = Mathf.RoundToInt(z.Clamp(0, grid.GetLength(1) - 1));

        int r = 0;
        while (true)
        {
            for(int i = -r; i <= r + r; i++)
            {
                for (int j = -r; j <= r; j++)
                {
                    if (i == -r || i == r || j == r || j == -r)
                    {
                        int xPos = xClamp + i;
                        int zPos = zClamp + j;
                        if (IsValidTile(xPos, zPos) && IsFree(xPos, zPos))
                        {
                            return new Point(xPos, zPos);
                        }
                    }
                }
            }
            r++;
        }
    }

    public bool IsFree(int x, int z)
    {
        return grid[x, z] <= Constants.Values.FOUND_FREE;
    }
    
    public List<Point> GetWalkableAdjacentPoints(Point p)
    {
        List<Point> toCheck = new List<Point>
        {
            new Point(p.x - 1, p.z),
            new Point(p.x + 1, p.z),
            new Point(p.x, p.z - 1),
            new Point(p.x, p.z + 1)
        };

        List<Point> validPoints = new List<Point>();

        foreach (Point check in toCheck)
        {
            if (IsValidTile(check.x, check.z))
            {
                if (grid[check.x, check.z] <= Constants.Values.FOUND_FREE)
                {
                    validPoints.Add(check);
                }
            }
        }
        return validPoints;
    }

    protected bool IsValidTile(int x, int z)
    {
        return x >= 0 && z >= 0 && x < grid.GetLength(0) && z < grid.GetLength(1);
    }
}
