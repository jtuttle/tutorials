﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeSphere : MonoBehaviour {
    public int gridSize;
    public float radius;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _normals;

    private Color32[] _cubeUV;

    private void Awake() {
        Generate();
    }

    private void Generate() {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Procedural Sphere";

        CreateVertices();
        CreateTriangles();
        CreateColliders();
    }

    private void CreateVertices() {
        int cornerVertices = 8;
        int edgeVertices = (gridSize + gridSize + gridSize - 3) * 4;
        int faceVertices = (
            (gridSize - 1) * (gridSize - 1) +
            (gridSize - 1) * (gridSize - 1) +
            (gridSize - 1) * (gridSize - 1)) * 2;
        
        _vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        _normals = new Vector3[_vertices.Length];
        _cubeUV = new Color32[_vertices.Length];

        int v = 0;
        for(int y = 0; y <= gridSize; y++) { 
            for(int x = 0; x <= gridSize; x++) {
                SetVertex(v++, x, y, 0);
            }
            for(int z = 1; z <= gridSize; z++) {
                SetVertex(v++, gridSize, y, z);
            }
            for(int x = gridSize - 1; x >= 0; x--) {
                SetVertex(v++, x, y, gridSize);
            }
            for(int z = gridSize - 1; z > 0; z--) {
                SetVertex(v++, 0, y, z);
            }
        }

        for(int z = 1; z < gridSize; z++) {
            for(int x = 1; x < gridSize; x++) {
                SetVertex(v++, x, gridSize, z);
            }
        }
        for(int z = 1; z < gridSize; z++) {
            for(int x = 1; x < gridSize; x++) {
                SetVertex(v++, x, 0, z);
            }
        }

        _mesh.vertices = _vertices;
        _mesh.normals = _normals;
        _mesh.colors32 = _cubeUV;
    }

    private void SetVertex(int i, int x, int y, int z) {
        Vector3 v = new Vector3(x, y, z) * 2f / gridSize - Vector3.one;

        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;

        Vector3 s;
        s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

        _normals[i] = s;
        _vertices[i] = _normals[i] * radius;
        _cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    private void CreateTriangles() {
        int[] trianglesZ = new int[(gridSize * gridSize) * 12];
        int[] trianglesX = new int[(gridSize * gridSize) * 12];
        int[] trianglesY = new int[(gridSize * gridSize) * 12];

        int quads = (gridSize * gridSize + gridSize * gridSize + gridSize * gridSize) * 2;
        //int[] tris = new int[quads * 6];

        int ring = (gridSize + gridSize) * 2;
        int tZ = 0, tX = 0, tY = 0, v = 0;

        for(int y = 0; y < gridSize; y++, v++) {
            for(int q = 0; q < gridSize; q++, v++) {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for(int q = 0; q < gridSize; q++, v++) {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for(int q = 0; q < gridSize; q++, v++) {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for(int q = 0; q < gridSize - 1; q++, v++) {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
        }

        tY = CreateTopFace(trianglesY, tY, ring);
        tY = CreateBottomFace(trianglesY, tY, ring);

        _mesh.subMeshCount = 3;
        _mesh.SetTriangles(trianglesZ, 0);
        _mesh.SetTriangles(trianglesX, 1);
        _mesh.SetTriangles(trianglesY, 2);
    }

    private int CreateTopFace(int[] tris, int t, int ring) {
        int v = ring * gridSize;
        for(int x = 0; x < gridSize - 1; x++, v++) {
            t = SetQuad(tris, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(tris, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (gridSize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for(int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++) {
            t = SetQuad(tris, t, vMin, vMid, vMin - 1, vMid + gridSize - 1);
            for(int x = 1; x < gridSize - 1; x++, vMid++) {
                t = SetQuad(tris, t, vMid, vMid + 1, vMid + gridSize - 1, vMid + gridSize);
            }
            t = SetQuad(tris, t, vMid, vMax, vMid + gridSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;

        t = SetQuad(tris, t, vMin, vMid, vTop + 1, vTop);
        for(int x = 1; x < gridSize - 1; x++, vTop--, vMid++) {
            t = SetQuad(tris, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(tris, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] tris, int t, int ring) {
        int v = 1;
        int vMid = _vertices.Length - (gridSize - 1) * (gridSize - 1);

        t = SetQuad(tris, t, ring - 1, vMid, 0, 1);
        for(int x = 1; x < gridSize - 1; x++, v++, vMid++) {
            t = SetQuad(tris, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(tris, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= gridSize - 2;
        int vMax = v + 2;

        for(int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++) {
            t = SetQuad(tris, t, vMin, vMid + gridSize - 1, vMin + 1, vMid);
            for(int x = 1; x < gridSize - 1; x++, vMid++) {
                t = SetQuad(tris, t, vMid + gridSize - 1, vMid + gridSize, vMid, vMid + 1);
            }
            t = SetQuad(tris, t, vMid + gridSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;

        t = SetQuad(tris, t, vTop + 1, vTop, vTop + 2, vMid);
        for(int x = 1; x < gridSize - 1; x++, vTop--, vMid++) {
            t = SetQuad(tris, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(tris, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
    }

    private int SetQuad(int[] tris, int i, int v00, int v10, int v01, int v11) {
        tris[i] = v00;
        tris[i + 1] = tris[i + 4] = v01;
        tris[i + 2] = tris[i + 3] = v10;
        tris[i + 5] = v11;
        return i + 6;
    }

    private void CreateColliders() {
        gameObject.AddComponent<SphereCollider>();
    }

    private void OnDrawGizmosSelected() {
        if(_vertices == null) return;

        Gizmos.color = Color.black;
        for(int i = 0; i < _vertices.Length; i++) {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(transform.TransformPoint(_vertices[i]), 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.TransformPoint(_vertices[i]), _normals[i]);
        }
    }
}
