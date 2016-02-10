using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {
    public Material material;

    public Mesh[] meshes;
    public int maxDepth;
    public float childScale;
    public float spawnProbability;
    public float maxRotationSpeed;
    public float maxTwist;

    private int _depth;
    private float _rotationSpeed;

    private static Vector3[] _childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] _childOrientations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0),
        Quaternion.Euler(-90f, 0f, 0)
    };

    private Material[] _materials;

    private void Start() {
        if(_materials == null)
            InitializeMaterials();

        gameObject.AddComponent<MeshFilter>().mesh = 
            meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = _materials[_depth];

        _rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);

        if(_depth < maxDepth) {
            StartCoroutine(CreateChildren());
        }
    }

    private void Update() {
        transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
    }

    private void InitializeMaterials() {
        _materials = new Material[maxDepth + 1];

        for(int i = 0; i <= maxDepth; i++) {
            _materials[i] = new Material(material);
            _materials[i].color =
                Color.Lerp(Color.white, Color.yellow, (float)i / maxDepth);
        }
    }

    private void Initialize(Fractal parent,
                            Vector3 direction,
                            Quaternion orientation) {
        meshes = parent.meshes;
        maxDepth = parent.maxDepth;
        childScale = parent.childScale;
        spawnProbability = parent.spawnProbability;
        maxRotationSpeed = parent.maxRotationSpeed;
        maxTwist = parent.maxTwist;
        _depth = parent._depth + 1;
        _materials = parent._materials;

        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = direction * (0.5f + 0.5f * childScale);
        transform.localRotation = orientation;
    }

    private IEnumerator CreateChildren() {
        for(int i = 0; i < _childDirections.Length; i++) {
            if(Random.value < spawnProbability) {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

                new GameObject("Fractal Child").AddComponent<Fractal>().
                    Initialize(this, _childDirections[i], _childOrientations[i]);
            }
        }
    }
}
