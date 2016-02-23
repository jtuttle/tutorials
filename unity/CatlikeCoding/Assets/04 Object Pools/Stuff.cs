using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stuff : PooledObject {
    public Rigidbody Body { get; private set; }

    MeshRenderer[] _meshRenderers;

    public void SetMaterial(Material m) {
        foreach(MeshRenderer meshRenderer in _meshRenderers) {
            meshRenderer.material = m;
        }
    }

    void Awake() {
        Body = GetComponent<Rigidbody>();
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    void OnTriggerEnter(Collider enteredCollider) {
        if(enteredCollider.CompareTag("Kill Zone"))
            ReturnToPool();
    }

    void OnLevelWasLoaded() {
        ReturnToPool();
    }
}
