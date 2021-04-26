using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : TerrainTile
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override int GetValue()
    {
        return 0;
    }
}
