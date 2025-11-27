using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Resource _resourcePrefab;

    private float _spawnDelayMin;
    private float _spawnDelayMax;
    private float _startSpawnDelay;
    private float _waitStep = 0.1f;

    private WaitForSeconds _spawnWait;

    private void Awake()
    {
        _spawnWait = new WaitForSeconds(_waitStep);
    }

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), _startSpawnDelay, _spawnDelayMin);
    }

    public void Initialize(float spawnDelayMin, float spawnDelayMax, float startSpawnDelay)
    {
        _spawnDelayMin = spawnDelayMin;
        _spawnDelayMax = spawnDelayMax;
        _startSpawnDelay = startSpawnDelay;
    }

    private IEnumerator RepeatingSpawn()
    {
        Spawn();

        float spawnDelay = Random.Range(_spawnDelayMin, _spawnDelayMax);

        yield return _spawnWait;
    }

    private void Spawn()
    {
        Instantiate(_resourcePrefab, _spawnPoints[Random.Range(0, _spawnPoints.Length-1)].position, Quaternion.identity, transform);
    }
}
