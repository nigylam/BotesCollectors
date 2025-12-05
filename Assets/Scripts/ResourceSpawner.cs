using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private SpawnZone[] _spawnZones;
    [SerializeField] private Resource _resourcePrefab;

    private float _spawnDelayMin;
    private float _spawnDelayMax;
    private float _startSpawnDelay;
    private float _waitStep = 0.1f;
    private float _spawnZonePositionOffset = 0.5f;

    private WaitForSeconds _spawnWait;
    private WaitForSeconds _startSpawnDelayWait;

    private void Awake()
    {
        _spawnWait = new WaitForSeconds(_waitStep);
        _startSpawnDelayWait = new WaitForSeconds(_startSpawnDelay);
    }

    private void Start()
    {
        StartCoroutine(RepeatingSpawn());
    }

    public void Initialize(float spawnDelayMin, float spawnDelayMax, float startSpawnDelay)
    {
        _spawnDelayMin = spawnDelayMin;
        _spawnDelayMax = spawnDelayMax;
        _startSpawnDelay = startSpawnDelay;
    }

    private IEnumerator RepeatingSpawn()
    {
        yield return _startSpawnDelayWait;

        while (enabled)
        {
            Spawn();

            float spawnDelay = UnityEngine.Random.Range(_spawnDelayMin, _spawnDelayMax);
            int numberIterations = Convert.ToInt32(spawnDelay / _waitStep);

            for (int i = 0; i < numberIterations; i++)
                yield return _spawnWait;
        }
    }

    private void Spawn()
    {
        SpawnZone spawnZone = GetSpawnZone();

        if (spawnZone == null)
            return;

        Vector3 spawnZonePosition = spawnZone.transform.position;

        Vector3 position = new Vector3(UnityEngine.Random.Range(spawnZonePosition.x - _spawnZonePositionOffset, spawnZonePosition.x + _spawnZonePositionOffset), spawnZonePosition.y, UnityEngine.Random.Range(spawnZonePosition.z - _spawnZonePositionOffset, spawnZonePosition.z + _spawnZonePositionOffset));

        spawnZone.SetResource(Instantiate(_resourcePrefab, position, Quaternion.identity, transform));
    }

    private SpawnZone GetSpawnZone()
    {
        SpawnZone spawnZone = null;
        List<SpawnZone> spawnZones = GetFreeZones();

        if (spawnZones.Count > 0)
        {
            int spawnZoneNumber = UnityEngine.Random.Range(0, spawnZones.Count - 1);
            spawnZone = spawnZones[spawnZoneNumber];
        }

        return spawnZone;
    }

    private List<SpawnZone> GetFreeZones()
    {
        List<SpawnZone> freeZones = new();

        foreach (var zone in _spawnZones)
        {
            if (zone.IsTaken == false)
                freeZones.Add(zone);
        }

        return freeZones;
    }
}