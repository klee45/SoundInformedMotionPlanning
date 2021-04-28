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

    protected override void SetDefaultColor()
    {
        materialRenderer.material.color = Constants.Colors.FLOOR_DEFAULT;
    }

    protected override int SetLayerOn()
    {
        return Constants.Layers.FLOOR_ON;
    }

    protected override int SetLayerOff()
    {
        return Constants.Layers.FLOOR_OFF;
    }
}
