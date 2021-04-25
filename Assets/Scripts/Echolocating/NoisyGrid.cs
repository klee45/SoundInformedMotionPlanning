using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisyGrid : MonoBehaviour
{
    [SerializeField]
    private GridVisualizer visualizer;

    private float[,] grid;
    
    private void Start()
    {
        grid = new float[WallContainer.instance.GetHeight(), WallContainer.instance.GetWidth()];

        for (int z = 0; z < grid.GetLength(1); z++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, z] = Random.Range(Constants.Values.FOUND_FREE, Constants.Values.FOUND_WALL);
            }
        }
        Redraw();
    }

    public float GetCellValue(Point p)
    {
        return grid[p.x, p.z];
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
            if (IsValidTile(check.x, check.x))
            {
                if (grid[check.x, check.z] <= Constants.Values.FOUND_FREE)
                {
                    validPoints.Add(check);
                }
            }
        }
        return validPoints;
    }

    public void UpdateMap(int x, int z, float uncertainty)
    {
        int r = Constants.Values.NOISY_MAP_RANGE;
        for (int h = -r; h <= r; h++)
        {
            for (int w = -r; w <= r; w++)
            {
                int xPos = x + w;
                int zPos = z + h;
                if (IsValidTile(xPos, zPos))
                {
                    float diff = WallContainer.instance.GetTrue(xPos, zPos) - grid[xPos, zPos];
                    grid[xPos, zPos] += diff * uncertainty * Constants.Values.LEARNING_RATE;
                }
                else
                {
                    //Debug.LogWarning(string.Format("Invalid tile ({0}, {1})", xPos, zPos));
                }
            }
        }
        Redraw();
        Print();
    }

    private bool IsValidTile(int x, int z)
    {
        return x >= 0 && z >= 0 && x < grid.GetLength(0) && z < grid.GetLength(1);
    }

    private void Redraw()
    {
        visualizer.Draw(grid);
    }

    private void Print()
    {
        string str = "";
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                str += string.Format("{0:0.00}\t", grid[x, z]);
            }
            str += "\n\n";
        }
        Debug.Log(str);
    }
}
