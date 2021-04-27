using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisyGrid : TerrainGrid
{
    [SerializeField]
    private GridVisualizer visualizer;
    
    private void Start()
    {
        grid = new float[TerrainContainer.instance.GetWidth(), TerrainContainer.instance.GetHeight()];

        for (int z = 0; z < grid.GetLength(1); z++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, z] = Random.Range(Constants.Values.FOUND_FREE, Constants.Values.FOUND_WALL);
            }
        }
        Redraw();
    }

    public void SetFree(int x, int z)
    {
        grid[x, z] = 0;
        TerrainContainer.instance.GetTerrain(x, z).TurnOffPermanent();
    }

    public void UpdateMap(TerrainTile terrain, float uncertainty)
    {
        int r = Constants.Values.NOISY_MAP_RANGE;
        for (int h = -r; h <= r; h++)
        {
            for (int w = -r; w <= r; w++)
            {
                int xPos = terrain.GetX() + w;
                int zPos = terrain.GetZ() + h;
                if (IsValidTile(xPos, zPos))
                {
                    float diff = TerrainContainer.instance.GetGrid().GetTrue(xPos, zPos) - grid[xPos, zPos];
                    float increase = diff * uncertainty * Constants.Values.LEARNING_RATE;
                    float result = grid[xPos, zPos] + increase;
                    grid[xPos, zPos] = result.Clamp(0, 1);
                    float val = grid[xPos, zPos];
                    if (val <= Constants.Values.FOUND_FREE || val >= Constants.Values.FOUND_WALL)
                    {
                        terrain.TurnOffPermanent();
                    }
                    else
                    {
                        terrain.TurnOff();
                    }
                }
                else
                {
                    //Debug.LogWarning(string.Format("Invalid tile ({0}, {1})", xPos, zPos));
                }
            }
        }
        Redraw();
        if (Constants.Debug.SHOW_ECHOLOCATION_MESSAGES)
        {
            Print();
        }
    }

    private void Redraw()
    {
        visualizer.Draw(this);
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
