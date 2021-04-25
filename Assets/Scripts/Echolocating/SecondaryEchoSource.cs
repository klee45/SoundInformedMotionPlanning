using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryEchoSource : EchoSource
{
    private PrimaryEchoSource initialSource;

    protected override void Awake()
    {
        base.Awake();
        int x = Mathf.RoundToInt(transform.localPosition.x);
        int z = Mathf.RoundToInt(transform.localPosition.z);
        this.name = string.Format("Secondary ({0}, {1})", x, z);
        var react = gameObject.AddComponent<ReactsToEchoNode>();
        react.Setup(this);
    }

    public void React(EchoSource source)
    {
        if (!RaycastHelper(source.transform.localPosition, source.name, out RaycastHit hit))
        {
            this.initialSource = source.GetPrimarySource();
            Echolocator.instance.AddDiffractionCheck(this, source);
            TurnOff();
        }
    }

    public void CheckPathToSource(EchoSource source)
    {
        SecondaryWave(source);
    }

    public void SecondaryWave(EchoSource source)
    {
        float dist = (source.gameObject.transform.localPosition - transform.localPosition).magnitude;
        strength = source.GetStrength() - dist;
        if (strength > 0)
        {
            Emit();
        }
    }

    protected override void PostRaycastSuccess(Wall wall)
    {
        int wallX = wall.GetX();
        int wallZ = wall.GetZ();
        float uncertainty = GetBaseUncertainty(wallX, wallZ);
        uncertainty = UpdateUncertainty(uncertainty, wall);
        initialSource.GetGrid().UpdateMap(wallX, wallZ, uncertainty);
    }

    private float UpdateUncertainty(float uncertainty, Wall wall)
    {
        Vector3 wallPos = wall.transform.localPosition;
        Vector3 mainSourcePos = initialSource.transform.localPosition;
        Vector3 direction = mainSourcePos - wallPos;
        Debug.Log(direction);

        int bitmap = (1 << Constants.Layers.WALL_ON | 1 << Constants.Layers.WALL_OFF);

        RaycastHit[] hits = Physics.RaycastAll(wallPos, direction.normalized, direction.magnitude, bitmap);
        float mod = Mathf.Pow(1 + hits.Length, 2);
        return GetBaseUncertainty(wall.GetX(), wall.GetZ()) / mod;
    }

    public override PrimaryEchoSource GetPrimarySource()
    {
        return initialSource;
    }
}
