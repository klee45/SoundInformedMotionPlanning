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

    protected override void SetColorOn()
    {
        materialRenderer.material.color = Constants.Colors.FLOOR_ON;
    }

    protected override void SetColorOff()
    {
        materialRenderer.material.color = Constants.Colors.FLOOR_OFF;
    }

    protected override void SetColorOffPermanent()
    {
        materialRenderer.material.color = Constants.Colors.FLOOR_TOTAL_OFF;
    }

}
