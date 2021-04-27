using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstimateVisual : MonoBehaviour
{
    [SerializeField]
    private float size = 2f;

    private Renderer materialRenderer;

    private void Awake()
    {
        materialRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        transform.localScale = new Vector3(size, transform.localScale.y, size);
        materialRenderer.enabled = true;
        Color c = materialRenderer.material.color;
        materialRenderer.material.color = new Color(1, 0, 0, 0.3f);
    }

    public void UpdatePosition(ExtendedKalmanFilter.State state)
    {
        Vector2 pos = state.GetSourcePos();
        transform.localPosition = new Vector3(pos.x, 0.5f, pos.y);
    }
}
