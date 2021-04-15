using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    private SoundSource source;
    private float size;
    private Renderer materialRenderer;

    private const float opacityMod = 2f;

    [SerializeField]
    private float duration = Constants.Values.WAVE_TIME;

    private void Awake()
    {
        materialRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        Color c = materialRenderer.material.color;
        materialRenderer.material.color = new Color(c.r, c.g, c.b, 1 / opacityMod);
    }

    public void Setup(SoundSource source, int x, int z, float size)
    {
        this.source = source;
        transform.localPosition = new Vector3(x, 0, z);
        this.size = size;
        transform.localScale = new Vector3(size, 0.1f, size);
    }

    void Update()
    {
        duration -= Time.deltaTime;
        Color c = materialRenderer.material.color;
        if (duration <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            materialRenderer.material.color = new Color(c.r, c.g, c.b, duration / Constants.Values.WAVE_TIME / opacityMod);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger enter with " + other.gameObject);
        other.GetComponent<ReactsToSound>().React(source);
    }
}
