﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GridVisualizer visualizer;
    [SerializeField]
    private int width, height;
    [SerializeField]
    private TextAsset text;

    private TerrainTile[,] terrain;

    private void Start()
    {
        if (text == null)
        {
            //GenerateFromGameObjects();
        }
        else
        {
            DisableInitialGameObjects();
            GenerateFromText();
        }

        grid.Print();
        visualizer.Draw(grid);

        /*
        Heap<string> h = new Heap<string>();
        h.Add("a", 1);
        h.Add("b", 5);
        h.Add("c", 0);
        h.Add("d", 10);
        h.Add("e", 6);
        h.Add("f", 9);
        h.Add("g", 12);
        h.Add("h", -1);
        h.Add("i", 3);
        h.Add("j", 5);

        //h.Print();
        while (h.HasValues())
        {
            Heap<string>.HeapPair<string> pair = h.Remove();
            Debug.Log(string.Format("{0}: {1}", pair.GetItem(), pair.GetValue()));
            //h.Print();
        }
        */
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

    /*
    private void GenerateFromGameObjects()
    {
        grid = new float[height, width];
        foreach (Transform child in transform)
        {
            TerrainTile terrain = child.GetComponent<TerrainTile>();
            grid[terrain.GetX(), terrain.GetZ()] = terrain.GetValue();
            //this.terrain.Add(terrain);
        }
    }
    */

    private void GenerateFromText()
    {
        List<List<int>> arr = new List<List<int>>();

        string[] lines = text.text.Split("\n"[0]);
        int width = 0;
        int height = lines.Length;
        for (int i = height - 1; i >= 0; i--)
        {
            string[] characters = lines[i].Split(' ');
            int length = characters.Length;

            List<int> lst = new List<int>();
            for (int j = 0; j < length; j++)
            {
                // Debug.Log(character);
                lst.Add(int.Parse(characters[j]));
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
                    case 0:
                        tile = CreateFloor(x, z);
                        val = 0;
                        break;
                    case 1:
                        tile = CreateWall(x, z);
                        val = 1;
                        break;
                    case 2:
                        tile = CreateFloor(x, z);
                        val = 0;
                        CreateSecondarySoundSource(x, z);
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
