using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryEchoSource : EchoSource
{
    [SerializeField]
    private NoisyGrid grid;

    private float delay = 1f;

    protected override void Awake()
    {
        base.Awake();
        grid = GetComponent<NoisyGrid>();
    }

    void Start()
    {
        //StartCoroutine(Wait(3));
    }

    private void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
        {
            delay = Constants.Values.ENABLE_TIME + 1.0f + delay;
            Emit();
        }
    }

    public NoisyGrid GetGrid()
    {
        return grid;
    }

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        Emit();
    }

    protected override void PostRaycastSuccess(Wall wall)
    {
        int wallX = wall.GetX();
        int wallZ = wall.GetZ();
        float uncertainty = GetBaseUncertainty(wallX, wallZ);
        grid.UpdateMap(wallX, wallZ, uncertainty);
    }

    public override PrimaryEchoSource GetPrimarySource()
    {
        return this;
    }
}
