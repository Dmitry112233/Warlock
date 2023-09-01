using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSmoothCorners : MonoBehaviour
{
    public float cornerRadius = 0.2f;

    private Mesh originalMesh;
    private Mesh modifiedMesh;

    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().mesh;
        modifiedMesh = new Mesh();

        UpdateMesh();
        GetComponent<MeshFilter>().mesh = modifiedMesh;
    }

    void UpdateMesh()
    {
        Vector3[] vertices = originalMesh.vertices;
        Vector3[] modifiedVertices = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            modifiedVertices[i] = RoundedCorner(vertices[i]);
        }

        modifiedMesh.vertices = modifiedVertices;
        modifiedMesh.triangles = originalMesh.triangles;
        modifiedMesh.normals = originalMesh.normals;
        modifiedMesh.uv = originalMesh.uv;

        modifiedMesh.RecalculateBounds();
    }

    Vector3 RoundedCorner(Vector3 vertex)
    {
        Vector3 corner = vertex;

        if (cornerRadius > 0)
        {
            corner = corner.normalized * (corner.magnitude - cornerRadius);
        }

        return corner;
    }
}
