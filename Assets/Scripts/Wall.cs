using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, ICanDisable
{
    private int x;
    private int z;

    private Renderer materialRenderer;

    private void Awake()
    {
        materialRenderer = GetComponent<Renderer>();

        var react = gameObject.AddComponent<ReactsToSoundWall>();
        react.Setup(this);
        Vector3 localPosition = transform.localPosition;
        this.x = (int)localPosition.x;
        this.z = (int)localPosition.z;
        this.name = string.Format("Wall ({0}, {1})", this.x, this.z);
    }

    public int GetX() { return x; }
    public int GetZ() { return z; }

    public void React(EchoSource source)
    {
        Echolocator.instance.AddWallCheck(this, source);
    }

    public void TurnOn()
    {
        StartCoroutine(DelayOn());
    }

    private IEnumerator DelayOn()
    {
        yield return new WaitForSeconds(Constants.Values.ENABLE_TIME);
        materialRenderer.material.color = Constants.Colors.WALL_ON;
        gameObject.layer = Constants.Layers.WALL_ON;
    }

    public void TurnOff()
    {
        materialRenderer.material.color = Constants.Colors.WALL_OFF;
        gameObject.layer = Constants.Layers.WALL_OFF;
        Echolocator.instance.AddToDisableList(this);
    }
}
