using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueGrid : TerrainGrid
{

    public float GetTrue(int x, int z)
    {
        return grid[x, z];
    }

    public void Print()
    {
        string str = "";
        for (int z = 0; z < grid.GetLength(1); z++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                str += string.Format("{0}\t", grid[x, z] == 1 ? "O" : ".");
            }
            str += "\n\n";
        }
        Debug.Log(str);
    }
}
