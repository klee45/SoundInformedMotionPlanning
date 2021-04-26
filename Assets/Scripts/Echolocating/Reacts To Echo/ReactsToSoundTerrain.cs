using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactsToSoundTerrain : ReactsToEcho
{
    private TerrainTile terrain;

    public void Setup(TerrainTile terrain)
    {
        this.terrain = terrain;
    }

    public override void React(EchoSource source)
    {
        terrain.React(source);
    }
}
