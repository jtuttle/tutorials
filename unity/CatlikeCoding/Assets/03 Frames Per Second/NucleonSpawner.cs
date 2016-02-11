using UnityEngine;

public class NucleonSpawner : MonoBehaviour {
    public float timeBetweenSpawns;
    public float spawnDistance;
    public Nucleon[] nucleonPrefabs;

    float _timeSinceLastSpawn;

    void FixedUpdate() {
        _timeSinceLastSpawn += Time.deltaTime;

        if(_timeSinceLastSpawn > timeBetweenSpawns) {
            _timeSinceLastSpawn -= timeBetweenSpawns;
            SpawnNucleon();
        }
    }

    void SpawnNucleon() {
        Nucleon prefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
        Nucleon spawn = Instantiate<Nucleon>(prefab);
        spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
    }
}
