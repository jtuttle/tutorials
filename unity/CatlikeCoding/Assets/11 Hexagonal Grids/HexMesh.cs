using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {
    private Mesh _hexMesh;
    private List<Vector3> _vertices;
    private List<int> _tris;

    private void Awake() {
        GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
        _hexMesh.name = "Hex Mesh";
        _vertices = new List<Vector3>();
        _tris = new List<int>();
    }

    public void TriangulateCells(HexCell[] cells) {
        _hexMesh.Clear();
        _vertices.Clear();
        _tris.Clear();

        for(int i = 0; i < cells.Length; i++) {
            Triangulate(cells[i]);
        }

        _hexMesh.vertices = _vertices.ToArray();
        _hexMesh.triangles = _tris.ToArray();
        _hexMesh.RecalculateNormals();
    }

    private void Triangulate(HexCell cell) {
        Vector3 center = cell.transform.localPosition;

        for(int i = 0; i < 6; i++) {
            AddTriangle(
                center, 
                center + HexMetrics.corners[i], 
                center + HexMetrics.corners[i + 1]
            );
        }
    }

    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _tris.Add(vertexIndex);
        _tris.Add(vertexIndex + 1);
        _tris.Add(vertexIndex + 2);
    }
}
