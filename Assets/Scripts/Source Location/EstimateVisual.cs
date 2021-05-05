using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visualization class for the extended kalman filter
/// estimates
/// </summary>
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
        materialRenderer.material.color = new Color(0.3f, 0.3f, 1.0f, 0.1f);
    }

    public void UpdatePosition(ExtendedKalmanFilter.State state)
    {
        Vector2 pos = state.GetSourcePos();
        if (float.IsNaN(pos.x) || float.IsNaN(pos.y))
        {
            transform.localPosition = new Vector3(0, 1f, 0);
        }
        else
        {
            transform.localPosition = new Vector3(pos.x, 1f, pos.y);
        }
    }
}
