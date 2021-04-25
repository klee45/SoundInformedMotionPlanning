using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    // Start is called before the first frame update
    public void Draw(float[,] grid)
    {
        int width = grid.GetLength(1);
        int height = grid.GetLength(0);
        Texture2D texture = new Texture2D(width, height);
        GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        texture.filterMode = FilterMode.Point;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Color color = GetColorFromVal(grid[z, x]);
                texture.SetPixel(x, height - z - 1, color);
                //Debug.Log(string.Format("Setting pixel at {0} {1} and did color {2}", x, z, color));
            }
        }

        GetComponent<UnityEngine.UI.RawImage>().texture = texture;
        texture.Apply();
    }

    private Color GetColorFromVal(float value)
    {
        if (value < Constants.Values.FOUND_FREE)
        {
            return Constants.Colors.MAP_FREE;
        }
        else if (value > Constants.Values.FOUND_WALL)
        {
            return Constants.Colors.MAP_WALL;
        }
        else
        {
            return new Color(value, value, value);
        }
    }
}
