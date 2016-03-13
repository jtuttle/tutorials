﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoundedCube : MonoBehaviour {
    public int xSize, ySize, zSize;
    public int roundness;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _normals;

    private void Awake() {
        Generate();
    }

    private void Generate() {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Procedural Cube";

        CreateVertices();
        CreateTriangles();
    }

    private void CreateVertices() {
        int cornerVertices = 8;
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;
        
        _vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        _normals = new Vector3[_vertices.Length];

        int v = 0;
        for(int y = 0; y <= ySize; y++) { 
            for(int x = 0; x <= xSize; x++) {
                SetVertex(v++, x, y, 0);
            }
            for(int z = 1; z <= zSize; z++) {
                SetVertex(v++, xSize, y, z);
            }
            for(int x = xSize - 1; x >= 0; x--) {
                SetVertex(v++, x, y, zSize);
            }
            for(int z = zSize - 1; z > 0; z--) {
                SetVertex(v++, 0, y, z);
            }
        }

        for(int z = 1; z < zSize; z++) {
            for(int x = 1; x < xSize; x++) {
                SetVertex(v++, x, ySize, z);
            }
        }
        for(int z = 1; z < zSize; z++) {
            for(int x = 1; x < xSize; x++) {
                SetVertex(v++, x, 0, z);
            }
        }

        _mesh.vertices = _vertices;
        _mesh.normals = _normals;
    }

    private void SetVertex(int i, int x, int y, int z) {
        Vector3 inner = _vertices[i] = new Vector3(x, y, z);

        if(x < roundness) {
            inner.x = roundness;
        } else if(x > xSize - roundness) {
            inner.x = xSize - roundness;
        }

        if(y < roundness) {
            inner.y = roundness;
        } else if(y > ySize - roundness) {
            inner.y = ySize - roundness;
        }

        if(z < roundness) {
            inner.z = roundness;
        } else if(z > zSize - roundness) {
            inner.z = zSize - roundness;
        }

        _normals[i] = (_vertices[i] - inner).normalized;
        _vertices[i] = inner + _normals[i] * roundness;
    }

    private void CreateTriangles() {
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        int[] tris = new int[quads * 6];

        int ring = (xSize + zSize) * 2;
        int t = 0, v = 0;

        for(int y = 0; y < ySize; y++, v++) {
            for(int q = 0; q < ring - 1; q++, v++) {
                t = SetQuad(tris, t, v, v + 1, v + ring, v + ring + 1);
            }
            t = SetQuad(tris, t, v, v - ring + 1, v + ring, v + 1);
        }

        t = CreateTopFace(tris, t, ring);
        t = CreateBottomFace(tris, t, ring);

        _mesh.triangles = tris;
    }

    private int CreateTopFace(int[] tris, int t, int ring) {
        int v = ring * ySize;
        for(int x = 0; x < xSize - 1; x++, v++) {
            t = SetQuad(tris, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(tris, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for(int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
            t = SetQuad(tris, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
            for(int x = 1; x < xSize - 1; x++, vMid++) {
                t = SetQuad(tris, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
            }
            t = SetQuad(tris, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;

        t = SetQuad(tris, t, vMin, vMid, vTop + 1, vTop);
        for(int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
            t = SetQuad(tris, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(tris, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] tris, int t, int ring) {
        int v = 1;
        int vMid = _vertices.Length - (xSize - 1) * (zSize - 1);

        t = SetQuad(tris, t, ring - 1, vMid, 0, 1);
        for(int x = 1; x < xSize - 1; x++, v++, vMid++) {
            t = SetQuad(tris, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(tris, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= xSize - 2;
        int vMax = v + 2;

        for(int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
            t = SetQuad(tris, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
            for(int x = 1; x < xSize - 1; x++, vMid++) {
                t = SetQuad(tris, t, vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
            }
            t = SetQuad(tris, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;

        t = SetQuad(tris, t, vTop + 1, vTop, vTop + 2, vMid);
        for(int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
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

    private void OnDrawGizmos() {
        if(_vertices == null) return;

        Gizmos.color = Color.black;
        for(int i = 0; i < _vertices.Length; i++) {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(transform.TransformPoint(_vertices[i]), 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_vertices[i], _normals[i]);
        }
    }
}
