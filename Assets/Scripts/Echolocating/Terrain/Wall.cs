using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : TerrainTile
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override int GetValue()
    {
        return 1;
    }

    /*
    protected override void React(EchoSource source)
    {
        Echolocator.instance.AddWallCheck(this, source);
    }
    */
}
