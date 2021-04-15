using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoundSource : MonoBehaviour, ICanDisable
{
    [SerializeField]
    protected float strength;

    private Renderer materialRenderer;

    protected virtual void Awake()
    {
        gameObject.layer = Constants.Layers.SOURCE_ON;
        materialRenderer = GetComponent<Renderer>();
    }

    public void RaycastFromWallToSelf(Wall wall)
    {
        if (!RaycastHelper(wall.transform.localPosition, wall.name, out RaycastHit hit))
        {
            wall.TurnOff();
            PostRaycastSuccess(wall);
        }
    }

    protected bool RaycastHelper(Vector3 pos, string targetName, out RaycastHit hit)
    {
        Vector3 direction = transform.localPosition - pos;
        //Debug.Log(direction);

        int bitmap = (1 << Constants.Layers.WALL_ON | 1 << Constants.Layers.WALL_OFF);

        if (Physics.Raycast(pos, direction.normalized, out hit, direction.magnitude, bitmap))
        {
            Debug.Log("Raycast to " + this.name + " from " + targetName + " hit " + hit.collider.name);
            return true;
        }
        else
        {
            Debug.Log("Raycast to " + this.name + " from " + targetName + " success");
            return false;
        }
    }

    protected virtual void Emit()
    {
        gameObject.layer = Constants.Layers.SOURCE_OFF;
        EmitHelper();
    }

    protected void EmitHelper()
    {
        Vector3 pos = transform.localPosition;

        SoundWave wave = WaveManager.instance.CreateWave();
        wave.Setup(this, Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z), strength * 2);
        wave.transform.parent = this.transform;
    }

    protected float GetBaseUncertainty(int x, int z)
    {
        float xDiff = transform.localPosition.x - x;
        float zDiff = transform.localPosition.z - z;
        float dist = Mathf.Sqrt(xDiff * xDiff + zDiff * zDiff);
        float uncertainty = 1 - Mathf.Max(0, (strength - dist)) / strength;
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
        yield return new WaitForSeconds(Constants.Values.ENABLE_TIME);
        gameObject.layer = Constants.Layers.SOURCE_ON;
        materialRenderer.material.color = Constants.Colors.SOURCE_ON;
    }

    public virtual void TurnOff()
    {
        gameObject.layer = Constants.Layers.SOURCE_OFF;
        materialRenderer.material.color = Constants.Colors.SOURCE_OFF;
        Echolocator.instance.AddToDisableList(this);
    }

    public abstract PrimarySoundSource GetPrimarySource();
    protected abstract void PostRaycastSuccess(Wall wall);
}
