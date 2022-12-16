using UnityEngine;

public class AimLineRender : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private Vector3[] points;

    public void SetUpLine(Vector3[] points) 
    {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    private void Update()
    {
        if(points!= null && points.Length > 1) 
        {
            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }
        }
        else 
        {
            points = null;
            lineRenderer.positionCount = 0;
        }
    }
}
