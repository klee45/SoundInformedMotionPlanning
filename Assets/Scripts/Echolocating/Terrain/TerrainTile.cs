using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base behavior of any terrain
/// </summary>
public abstract class TerrainTile : MonoBehaviour, ICanDisable
{
    protected Renderer materialRenderer;

    private bool totalOff = false;

    protected int x;
    protected int z;

    protected virtual void Awake()
    {
        materialRenderer = GetComponent<Renderer>();

        var react = gameObject.AddComponent<ReactsToSoundTerrain>();
        react.Setup(this);
    }

    public abstract Vector3 GetLevelHeight();

    private void Start()
    {
        if (SceneVariables.instance.DO_COLOR_WALLS)
        {
            SetColorOn();
        }
        else
        {
            SetDefaultColor();
        }
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
        yield return new WaitForSeconds(Constants.Values.TERRAIN_DISABLED_DURATION);
        if (SceneVariables.instance.DO_COLOR_WALLS)
        {
            SetColorOn();
        }
        gameObject.layer = SetLayerOn();
    }

    protected abstract void SetDefaultColor();
    protected abstract void SetColorOn();
    protected abstract void SetColorOff();
    protected abstract void SetColorOffPermanent();

    protected abstract int SetLayerOn();
    protected abstract int SetLayerOff();

    public void TurnOff()
    {
        if (!totalOff)
        {
            if (SceneVariables.instance.DO_COLOR_WALLS)
            {
                SetColorOff();
            }
            gameObject.layer = SetLayerOff();
            Echolocator.instance.AddToDisableList(this);
        }
    }

    public void TurnOffPermanent()
    {
        if (SceneVariables.instance.DO_COLOR_WALLS)
        {
            SetColorOffPermanent();
        }
        gameObject.layer = SetLayerOff();
        totalOff = true;
    }
}
