using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTest : MonoBehaviour
{
    public LineRenderController line;

    public void DrawLine(Vector3[] points) 
    {
        line.SetUpLine(points);
    }
}
