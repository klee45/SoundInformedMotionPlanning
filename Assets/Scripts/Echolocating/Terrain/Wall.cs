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

    protected override void SetColorOn()
    {
        materialRenderer.material.color = Constants.Colors.WALL_ON;
    }

    protected override void SetColorOff()
    {
        materialRenderer.material.color = Constants.Colors.WALL_OFF;
    }

    protected override void SetColorOffPermanent()
    {
        materialRenderer.material.color = Constants.Colors.WALL_TOTAL_OFF;
    }

    /*
    protected override void React(EchoSource source)
    {
        Echolocator.instance.AddWallCheck(this, source);
    }
    */
}
