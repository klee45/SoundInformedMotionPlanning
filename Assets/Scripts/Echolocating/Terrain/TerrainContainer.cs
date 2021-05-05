using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all of the terrain and manages the visualization and
/// storage of the true grid information
/// 
/// Also manages map generation
/// View map.txt file to generate maps
/// 
/// 0 = floor
/// 1 = wall
/// 2 = secondary sound source (and floor)
/// p = player
/// r = robot
/// </summary>
public class TerrainContainer : Singleton<TerrainContainer>
{
    [SerializeField]
    private TrueGrid grid;

    [SerializeField]
    private GameObject floorContainer;
    [SerializeField]
    private GameObject wallContainer;
    [SerializeField]
    private GameObject secondaryEchoSourceContainer;

    [SerializeField]
    private Wall wallPrefab;
    [SerializeField]
    private Floor floorPrefab;
    [SerializeField]
    private SecondaryEchoSource secondaryEchoSourcePrefab;

    [SerializeField]
    private GameObject robot;
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GridVisualizer visualizer;
    [SerializeField]
    private int width, height;
    [SerializeField]
    private TextAsset text;

    private TerrainTile[,] terrain;

    private void Start()
    {
        if (floorContainer.transform.childCount > 0 ||
            wallContainer.transform.childCount > 0 ||
            secondaryEchoSourceContainer.transform.childCount > 0)
        {
            GenerateFromGameObjects();
        }
        else
        {
            DisableInitialGameObjects();
            GenerateFromText();
        }

        grid.Print();
        visualizer.Draw(grid);
    }

    private void DisableInitialGameObjects()
    {
        foreach (Transform child in transform)
        {
            GameObject obj = child.gameObject;
            if (obj != floorContainer &&
                obj != wallContainer &&
                obj != secondaryEchoSourceContainer)
            {
                Destroy(obj);
            }
        }
    }

    private void GenerateFromGameObjects()
    {
        terrain = new TerrainTile[width, height];
        grid.InitializeSize(width, height);
        foreach (TerrainTile tile in GetComponentsInChildren<TerrainTile>())
        {
            terrain[tile.GetX(), tile.GetZ()] = tile;
            grid.SetCellValue(tile.GetX(), tile.GetZ(), tile.GetValue());
            //this.terrain.Add(terrain);
        }
    }

    private void GenerateFromText()
    {
        List<List<char>> arr = new List<List<char>>();

        string[] lines = text.text.Split("\n"[0]);
        int width = 0;
        int height = lines.Length;
        for (int i = height - 1; i >= 0; i--)
        {
            string[] characters = lines[i].Split('\t');
            int length = characters.Length;

            List<char> lst = new List<char>();
            for (int j = 0; j < length; j++)
            {
                // Debug.Log(character);
                lst.Add(characters[j][0]);
            }
            if (length > width)
            {
                width = length;
            }
            arr.Add(lst);
        }
        grid.InitializeSize(width, height);
        terrain = new TerrainTile[width, height];
        this.width = width;
        this.height = height;

        int zLen = arr.Count;
        for (int z = 0; z < zLen; z++)
        {
            int xLen = arr[z].Count;
            for (int x = 0; x < xLen; x++)
            {
                // Because of Unity's direction system,
                // we have to switch x and z at some point
                int character = arr[z][x];
                int val;
                TerrainTile tile;
                switch (character)
                {
                    case '0':
                        tile = CreateFloor(x, z);
                        val = 0;
                        break;
                    case '1':
                        tile = CreateWall(x, z);
                        val = 1;
                        break;
                    case '2':
                        tile = CreateFloor(x, z);
                        val = 0;
                        CreateSecondarySoundSource(x, z);
                        break;
                    case 'p':
                        player.transform.localPosition = new Vector3(x, 0, z);
                        tile = CreateFloor(x, z);
                        val = 0;
                        break;
                    case 'r':
                        robot.transform.localPosition = new Vector3(x, 0, z);
                        tile = CreateFloor(x, z);
                        val = 0;
                        break;
                    default:
                        Debug.LogWarning("Map file has invalid character " + character);
                        val = 1;
                        tile = null;
                        break;
                }
                grid.SetCellValue(x, z, val);
                terrain[x, z] = tile;
            }
        }
    }

    private TerrainTile CreateFloor(int x, int z)
    {
        TerrainTile obj = CreateAtTile<TerrainTile>(x, z, floorPrefab, floorContainer);
        obj.transform.localPosition = new Vector3(x, -0.05f, z);
        obj.transform.localScale = new Vector3(1, 0.1f, 1);
        return obj;
    }

    private TerrainTile CreateWall(int x, int z)
    {
        TerrainTile obj = CreateAtTile<TerrainTile>(x, z, wallPrefab, wallContainer);
        obj.transform.localPosition = new Vector3(x, 1, z);
        obj.transform.localScale = new Vector3(1, 2, 1);
        return obj;
    }

    private void CreateSecondarySoundSource(int x, int z)
    {
        SecondaryEchoSource obj = CreateAtTile<SecondaryEchoSource>(x, z, secondaryEchoSourcePrefab, secondaryEchoSourceContainer);
        obj.transform.localPosition = new Vector3(x, 1, z);
    }

    private T CreateAtTile<T>(int x, int z, T prefab, GameObject container) where T : MonoBehaviour
    {
        T obj = Instantiate(prefab);
        obj.transform.parent = container.transform;
        obj.name = string.Format("{2} ({0}, {1})", x, z, prefab.name);
        return obj;
    }

    public int GetWidth() { return width; }
    public int GetHeight() { return height; }

    public TerrainTile GetTerrain(int x, int z)
    {
        return terrain[x, z];
    }

    public TrueGrid GetGrid()
    {
        return grid;
    }
}
