﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour {
    public int xSize, ySize;
    public bool delayedRender = true;

    private Vector3[] _vertices;
    private Mesh _mesh;

    private void Awake() {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate() {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Procedural Grid";

        _vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[_vertices.Length];
        Vector4[] tangents = new Vector4[_vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        for(int i = 0, y = 0; y <= ySize; y++) {
            for(int x = 0; x <= xSize; x++, i++) {
                _vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;

                if(delayedRender)
                    yield return wait;
            }
        }

        _mesh.vertices = _vertices;
        _mesh.uv = uv;
        _mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];

        for(int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
            for(int x = 0; x < xSize; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;

                _mesh.triangles = triangles;

                if(delayedRender)
                    yield return wait;
            }
        }

        _mesh.RecalculateNormals();
    }

    private void OnDrawGizmos() {
        if(_vertices == null) return;

        Gizmos.color = Color.black;
        for(int i = 0; i < _vertices.Length; i++) {
            Gizmos.DrawSphere(transform.TransformPoint(_vertices[i]), 0.1f);
        }
    }
}
