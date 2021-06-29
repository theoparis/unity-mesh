using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public int xSize = 20;
    public int zSize = 20;
    public FastNoiseLite.NoiseSettings noiseSettings = new FastNoiseLite.NoiseSettings();
    public float generationDelay = .01f;
    public float uvScaleFactor = 1f;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        StartCoroutine(CreateShape());
    }

    private void OnValidate()
    {
        StartCoroutine(CreateShape());
    }

    private void Update()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.SetUVs(0, uv);
        mesh.RecalculateNormals();
    }

    private IEnumerator CreateShape()
    {
        // Create vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        var noise = new FastNoiseLite(noiseSettings);

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (var x = 0; x <= xSize; x++)
            {
                var y = noise.GetNoise(x, z) * 2f;
                vertices[i++] = new Vector3(x, y, z);
            }
        }

        // Create triangles
        triangles = new int[xSize * zSize * 6];
        uv = new Vector2[(xSize + 1) * (zSize + 1)];

        for (int i = 0, d = 0; d <= zSize; d++)
        {
            for (var w = 0; w <= xSize; w++)
            {
                uv[i] = new Vector2(w / (float)xSize, d / (float)zSize);
                i++;
            }
        }

        var verts = 0;
        var tris = 0;
        for (var z = 0; z < zSize; z++)
        {
            for (var x = 0; x < xSize; x++)
            {
                // quad triangles index.
                int ti = (z * (xSize) + x) * 6; // 6 - polygons per quad * corners per polygon
                // First triangle
                triangles[ti] = (z * (xSize + 1)) + x;
                triangles[ti + 1] = ((z + 1) * (xSize + 1)) + x;
                triangles[ti + 2] = ((z + 1) * (xSize + 1)) + x + 1;
                // Second triangle
                triangles[ti + 3] = (z * (xSize + 1)) + x;
                triangles[ti + 4] = ((z + 1) * (xSize + 1)) + x + 1;
                triangles[ti + 5] = (z * (xSize + 1)) + x + 1;

                verts++;
                tris += 6;
                yield return new WaitForSeconds(generationDelay);
            }
            verts++;
        }
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        foreach (var v in vertices)
        {
            Gizmos.DrawSphere(v, .1f);
        }
    }
}
