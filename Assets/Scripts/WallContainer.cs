using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallContainer : Singleton<WallContainer>
{
    [SerializeField]
    private GridVisualizer visualizer;
    [SerializeField]
    private int width, height;

    private float[,] grid;

    private List<Wall> walls;
    
    private void Start()
    {
        grid = new float[height, width];
        walls = new List<Wall>();
        foreach (Transform child in transform)
        {
            Wall wall = child.GetComponent<Wall>();
            grid[wall.GetX(), wall.GetZ()] = 1;
            walls.Add(wall);
        }

        Print();
        visualizer.Draw(grid);
    }

    public int GetWidth() { return width; }
    public int GetHeight() { return height; }

    public float GetTrue(int z, int x)
    {
        return grid[z, x];
    }

    private void Print()
    {
        string str = "";
        for (int z = 0; z < grid.GetLength(0); z++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                str += string.Format("{0}\t", grid[z, x] == 1 ? "O" : ".");
            }
            str += "\n\n";
        }
        Debug.Log(str);
    }
}
