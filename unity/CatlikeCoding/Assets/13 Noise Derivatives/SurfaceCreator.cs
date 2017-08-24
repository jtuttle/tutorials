using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SurfaceCreator : MonoBehaviour {
    [Range(1, 200)]
    public int resolution = 10;

    public Vector3 offset;
    public Vector3 rotation;

    [Range(0f, 1f)]
    public float strength = 1f;

    public bool damping;

    public float frequency = 1f;

    [Range(1, 8)]
    public int octaves = 1;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Range(1, 3)]
    public int dimensions = 3;

    public NoiseMethodType type;

    public Gradient coloring;

    public bool coloringForStrength;

    public bool showNormals;

    private int _currentResolution;

    private Mesh _mesh;

    private Vector3[] _vertices;
    private Vector3[] _normals;
    private Color[] _colors;

    private void OnEnable() {
        if(_mesh == null) {
            _mesh = new Mesh();
            _mesh.name = "Surface Mesh";
            GetComponent<MeshFilter>().mesh = _mesh;
        }
        Refresh();
    }

    private void OnDrawGizmosSelected() {
        float scale = 1f / resolution;

        if(showNormals && _vertices != null) {
            Gizmos.color = Color.yellow;

            for(int i = 0; i < _vertices.Length; i++) {
                Gizmos.DrawRay(_vertices[i], _normals[i] * scale);
            }
        }
    }

    public void Refresh() {
        if(resolution != _currentResolution) {
            CreateGrid();
        }

        Quaternion q = Quaternion.Euler(rotation);
        Vector3 point00 = q * new Vector3(-0.5f,-0.5f) + offset;
        Vector3 point10 = q * new Vector3( 0.5f,-0.5f) + offset;
        Vector3 point01 = q * new Vector3(-0.5f, 0.5f) + offset;
        Vector3 point11 = q * new Vector3( 0.5f, 0.5f) + offset;

        NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];
        float stepSize = 1f / resolution;

        float amplitude = (damping ? strength / frequency : strength);

        for(int v = 0, y = 0; y <= resolution; y++) {
            Vector3 point0 = Vector3.Lerp(point00, point01, y * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, y * stepSize);

            for(int x = 0; x <= resolution; x++, v++) {
                Vector3 point = Vector3.Lerp(point0, point1, x * stepSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);

                // Doesn't looks like this is used for the surface creator...
                /*
                if(type != NoiseMethodType.Value) {
                    sample = sample * 0.5f + 0.5f;
                }
                */

                sample = (type == NoiseMethodType.Value ? sample - 0.5f : sample * 0.5f);

                if(coloringForStrength) {
                    _colors[v] = coloring.Evaluate(sample + 0.5f);
                    sample *= amplitude;
                } else {
                    sample *= amplitude;
                    _colors[v] = coloring.Evaluate(sample + 0.5f);
                }

                _vertices[v].y = sample;
            }
        }

        _mesh.vertices = _vertices;
        _mesh.colors = _colors;
        _mesh.RecalculateNormals();

        CalculateNormals();

        _normals = _mesh.normals;
    }

    private void CreateGrid() {
        _currentResolution = resolution;
        _mesh.Clear();

        _vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        _normals = new Vector3[_vertices.Length];
        _colors = new Color[_vertices.Length];
        Vector2[] uv = new Vector2[_vertices.Length];
        float stepSize = 1f / resolution;

        for(int v = 0, z = 0; z <= resolution; z++) {
            for(int x = 0; x <= resolution; x++, v++) {
                _vertices[v] = new Vector3(x * stepSize - 0.5f, 0f, z * stepSize - 0.5f);
                _normals[v] = Vector3.up;
                _colors[v] = Color.black;
                uv[v] = new Vector2(x * stepSize, z * stepSize);
            }
        }

        _mesh.vertices = _vertices;
        _mesh.normals = _normals;
        _mesh.colors = _colors;
        _mesh.uv = uv;

        int[] triangles = new int[6 * resolution * resolution];

        for(int t = 0, v = 0, y = 0; y < resolution; y++, v++) {
            for(int x = 0; x < resolution; x++, v++, t += 6) {
                triangles[t] = v;
                triangles[t + 1] = v + resolution + 1;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + resolution + 1;
                triangles[t + 5] = v + resolution + 2;
            }
        }

        _mesh.triangles = triangles;
    }

    private void CalculateNormals() {
        for(int v = 0, z = 0; z <= resolution; z++) {
            for(int x = 0; x <= resolution; x++, v++) {
                _normals[v] = Vector3.up * GetXDerivative(x, z);
            }
        }
    }

    private float GetXDerivative(float x, float z) {
        //int rowOffset = z * (resolution + 1);
        return 0;
    }
}
