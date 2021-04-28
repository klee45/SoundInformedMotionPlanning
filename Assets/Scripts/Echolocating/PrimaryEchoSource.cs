using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryEchoSource : EchoSource
{
    [SerializeField]
    private AudioSource echoSound;

    [SerializeField]
    private NoisyGrid grid;

    private float delay = 1f;

    private bool isActive = false;

    protected override void Awake()
    {
        base.Awake();
        grid = GetComponent<NoisyGrid>();
    }

    void Start()
    {
        //StartCoroutine(Wait(3));
    }

    public void Activate(bool val)
    {
        this.isActive = val;
    }

    private void Update()
    {
        if (isActive)
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                delay = Constants.Timing.SCAN_TIME + delay;
                Emit();
            }
        }
    }

    public NoisyGrid GetGrid()
    {
        return grid;
    }

    protected override void Emit()
    {
        echoSound.Play();
        Vector3 pos = transform.localPosition;
        grid.SetFree(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));

        base.Emit();
    }

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        Emit();
    }

    protected override void PostRaycastSuccess(TerrainTile terrain)
    {
        int terrainX = terrain.GetX();
        int terrainZ = terrain.GetZ();
        float uncertainty = GetBaseUncertainty(terrainX, terrainZ);
        grid.UpdateMap(terrain, uncertainty);
    }

    public override PrimaryEchoSource GetPrimarySource()
    {
        return this;
    }
}
