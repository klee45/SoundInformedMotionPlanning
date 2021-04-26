using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echolocator : Singleton<Echolocator>
{
    private Queue<TerrainCheck> wallChecks;
    private Queue<DiffractionCheck> diffractionChecks;

    private List<ICanDisable> disableList;

    [SerializeField]
    private int wallCheckCount;
    [SerializeField]
    private int diffractionCheckCount;

    private void Start()
    {
        wallChecks = new Queue<TerrainCheck>();
        diffractionChecks = new Queue<DiffractionCheck>();
        disableList = new List<ICanDisable>();
    }

    private void Update()
    {
        wallCheckCount = wallChecks.Count;
        diffractionCheckCount = diffractionChecks.Count;

        AllWallChecks();
        if (wallChecks.Count == 0)
        {
            AllDiffractionChecks();
        }
        if (diffractionChecks.Count == 0)
        {
            ResetAll();
        }
    }

    public bool IsFree()
    {
        return wallChecks.Count == 0 && diffractionChecks.Count == 0;
    }

    private void AllWallChecks()
    {
        int count = Mathf.CeilToInt(wallChecks.Count / Constants.Values.ECHOLOCATE_RATE);
        for (int i = 0; i < count; i++)
        {
            DoWallCheck(wallChecks.Dequeue());
        }
    }

    public void AddToDisableList(ICanDisable obj)
    {
        disableList.Add(obj);
    }

    private void AllDiffractionChecks()
    {
        int count = Mathf.CeilToInt(diffractionChecks.Count / Constants.Values.ECHOLOCATE_RATE);
        for (int i = 0; i < count; i++)
        {
            DoDiffractionCheck(diffractionChecks.Dequeue());
        }
    }

    private void DoWallCheck(TerrainCheck terrainCheck)
    {
        terrainCheck.source.RaycastFromTerrainToSelf(terrainCheck.terrain);
    }

    private void DoDiffractionCheck(DiffractionCheck diffractionCheck)
    {
        diffractionCheck.secondary.CheckPathToSource(diffractionCheck.source);
    }

    private void ResetAll()
    {
        foreach (ICanDisable obj in disableList)
        {
            obj.TurnOn();
        }
        disableList.Clear();
    }

    public void AddTerrainCheck(TerrainTile terrain, EchoSource source)
    {
        wallChecks.Enqueue(new TerrainCheck(terrain, source));
    }

    public void AddDiffractionCheck(SecondaryEchoSource secondary, EchoSource source)
    {
        diffractionChecks.Enqueue(new DiffractionCheck(secondary, source));
    }

    private readonly struct TerrainCheck
    {
        public readonly TerrainTile terrain;
        public readonly EchoSource source;
        public TerrainCheck(TerrainTile terrain, EchoSource source)
        {
            this.terrain = terrain;
            this.source = source;
        }
    }

    private readonly struct DiffractionCheck
    {
        public readonly SecondaryEchoSource secondary;
        public readonly EchoSource source;
        public DiffractionCheck(SecondaryEchoSource secondary, EchoSource source)
        {
            this.secondary = secondary;
            this.source = source;
        }
    }
}
