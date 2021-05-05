using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Small script to allow for visualizing a path using a linerenderer
/// </summary>
public class PathVisualization : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        this.lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawPath(List<Point> path, TerrainGrid grid)
    {
        int len = path.Count;
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < len; i++)
        {
            Point point = path[i];
            if (grid.IsFreeTile(point.x, point.z))
            {
                positions.Add(PointToVector3(point));
            }
            else // Cull path except for end after can't move
            {
                positions.Add(PointToVector3(path[len - 1]));
                break;
            }
        }
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    private Vector3 PointToVector3(Point p)
    {
        return new Vector3(p.x, 1f, p.z);
    }

    public void Show()
    {
        lineRenderer.enabled = true;
    }

    public void Hide()
    {
        lineRenderer.enabled = false;
    }
}
