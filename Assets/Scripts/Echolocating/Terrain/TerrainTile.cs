using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainTile : MonoBehaviour, ICanDisable
{
    private Renderer materialRenderer;

    private bool totalOff = false;

    protected int x;
    protected int z;

    protected virtual void Awake()
    {
        materialRenderer = GetComponent<Renderer>();

        var react = gameObject.AddComponent<ReactsToSoundTerrain>();
        react.Setup(this);
    }

    private void Start()
    {
        SetColorOn();
        DefinePosition();
    }

    public void DefinePosition()
    {
        Vector3 localPosition = transform.localPosition;
        this.x = (int)localPosition.x;
        this.z = (int)localPosition.z;
    }

    public int GetX() { return x; }
    public int GetZ() { return z; }

    public abstract int GetValue();

    public void React(EchoSource source)
    {
        Echolocator.instance.AddTerrainCheck(this, source);
    }

    public void TurnOn()
    {
        if (!totalOff)
        {
            StartCoroutine(DelayOn());
        }
    }

    private IEnumerator DelayOn()
    {
        yield return new WaitForSeconds(Constants.Values.ENABLE_TIME);
        SetColorOn();
        gameObject.layer = Constants.Layers.TERRAIN_ON;
    }

    private void SetColorOn()
    {
        materialRenderer.material.color = Constants.Colors.TERRAIN_ON;
    }

    public void TurnOff()
    {
        if (!totalOff)
        {
            materialRenderer.material.color = Constants.Colors.TERRAIN_OFF;
            gameObject.layer = Constants.Layers.TERRAIN_OFF;
            Echolocator.instance.AddToDisableList(this);
        }
    }

    public void TurnOffPermanent()
    {
        materialRenderer.material.color = Constants.Colors.TERRAIN_TOTAL_OFF;
        gameObject.layer = Constants.Layers.TERRAIN_OFF;
        totalOff = true;
    }
}
