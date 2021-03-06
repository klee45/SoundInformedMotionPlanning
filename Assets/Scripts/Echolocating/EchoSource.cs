using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for both kinds of echo sources
/// Controls raycast tests to self and echo emission
/// </summary>
public abstract class EchoSource : MonoBehaviour, ICanDisable
{
    [SerializeField]
    protected float strength;

    private Renderer materialRenderer;

    protected virtual void Awake()
    {
        gameObject.layer = Constants.Layers.SOURCE_ON;
        materialRenderer = GetComponent<Renderer>();
    }

    public void RaycastFromTerrainToSelf(TerrainTile terrain)
    {
        if (!RaycastHelper(terrain.transform.localPosition, terrain.name, out RaycastHit hit))
        {
            PostRaycastSuccess(terrain);
        }
    }

    protected bool RaycastHelper(Vector3 pos, string targetName, out RaycastHit hit)
    {
        Vector3 direction = transform.localPosition - pos;
        //Debug.Log(direction);

        int bitmap = (1 << Constants.Layers.WALL_ON | 1 << Constants.Layers.WALL_OFF);

        if (Physics.Raycast(pos, direction.normalized, out hit, direction.magnitude, bitmap))
        {
            if (SceneVariables.instance.SHOW_ECHOLOCATION_MESSAGES)
            {
                Debug.Log("Raycast to " + this.name + " from " + targetName + " hit " + hit.collider.name);
            }
            return true;
        }
        else
        {
            if (SceneVariables.instance.SHOW_ECHOLOCATION_MESSAGES)
            {
                Debug.Log("Raycast to " + this.name + " from " + targetName + " success");
            }
            return false;
        }
    }

    public virtual void Emit()
    {
        gameObject.layer = Constants.Layers.SOURCE_OFF;
        EmitHelper();
    }

    protected void EmitHelper()
    {
        Vector3 pos = transform.localPosition;

        EchoWave wave = WaveManager.instance.CreateWave();
        wave.Setup(this, Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z), strength * 2);
        wave.transform.parent = this.transform;
    }

    protected float GetBaseUncertainty(int x, int z)
    {
        float xDiff = transform.localPosition.x - x;
        float zDiff = transform.localPosition.z - z;
        float dist = Mathf.Sqrt(xDiff * xDiff + zDiff * zDiff);
        //Debug.Log(strength - dist);
        float uncertainty = Mathf.Max((strength - dist) / GetPrimarySource().strength, 0);
        //float uncertainty = 1 - Mathf.Min(Mathf.Max(0, (strength - dist)) / strength, 1);
        return uncertainty;
    }
    
    public float GetStrength()
    {
        return strength;
    }
    
    public void TurnOn()
    {
        StartCoroutine(DelayOn());
    }

    private IEnumerator DelayOn()
    {
        yield return new WaitForSeconds(Constants.Values.TERRAIN_DISABLED_DURATION);
        gameObject.layer = Constants.Layers.SOURCE_ON;
        materialRenderer.material.color = Constants.Colors.SOURCE_ON;
    }

    public virtual void TurnOff()
    {
        gameObject.layer = Constants.Layers.SOURCE_OFF;
        materialRenderer.material.color = Constants.Colors.SOURCE_OFF;
        Echolocator.instance.AddToDisableList(this);
    }

    public abstract PrimaryEchoSource GetPrimarySource();
    protected abstract void PostRaycastSuccess(TerrainTile terrain);
}
