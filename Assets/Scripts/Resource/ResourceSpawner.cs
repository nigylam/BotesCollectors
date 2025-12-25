using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnZone> _spawnZones;
    [SerializeField] private Resource _resourcePrefab;
    [SerializeField] private LayerMask _blockingLayers;

    private float _spawnDelayMin;
    private float _spawnDelayMax;
    private float _startSpawnDelay;
    private float _waitStep = 0.1f;
    private float _spawnZonePositionOffset = 0.5f;
    private float _resourceRadius = 1.5f;
    private float _maxSpawnAttempts = 5;
    private float _maxZoneAttempts = 5;

    private ObjectPool<Resource> _pool;
    private int _poolCapacity = 10;
    private int _poolMaxSize = 30;

    private WaitForSeconds _spawnWait;
    private WaitForSeconds _startSpawnDelayWait;

    private void Awake()
    {
        _spawnWait = new WaitForSeconds(_waitStep);
        _startSpawnDelayWait = new WaitForSeconds(_startSpawnDelay);

        _pool = new ObjectPool<Resource>(
            createFunc: () => Instantiate(_resourcePrefab),
            actionOnGet: (obj) => obj.gameObject.SetActive(true),
            actionOnRelease: (obj) => obj.gameObject.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj.gameObject),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize
        );
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

    public void ClearSpawnZones(Vector3 position, float radius)
    {
        foreach (var collider in Physics.OverlapSphere(position, radius))
        {
            if(collider.TryGetComponent(out SpawnZone spawnZone))
                _spawnZones.Remove(spawnZone);
        }
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
        for (int zoneAttempts = 0; zoneAttempts < _maxZoneAttempts; zoneAttempts++)
        {
            SpawnZone spawnZone = GetSpawnZone();

            if (spawnZone == null)
                return;

            Vector3 spawnZonePosition = spawnZone.transform.position;

            for (int i = 0; i < _maxSpawnAttempts; i++)
            {

                Vector3 position = new
                (
                    UnityEngine.Random.Range
                    (
                        spawnZonePosition.x - _spawnZonePositionOffset, 
                        spawnZonePosition.x + _spawnZonePositionOffset
                    ),

                    spawnZonePosition.y,

                    UnityEngine.Random.Range
                    (
                        spawnZonePosition.z - _spawnZonePositionOffset, 
                        spawnZonePosition.z + _spawnZonePositionOffset
                    )
                 );

                if (IsSpawnAllowed(position))
                {
                    SpawnAt(position, spawnZone);
                    return;
                }
            }
        }
    }

    private void SpawnAt(Vector3 position, SpawnZone spawnZone)
    {
        Resource resource = _pool.Get();
        resource.Released += Release;
        resource.transform.SetParent(transform);
        resource.transform.position = position;
        spawnZone.SetResource(resource);
    }

    private bool IsSpawnAllowed(Vector3 position)
    {
        if (Physics.OverlapSphere(position, _resourceRadius, _blockingLayers).Length > 0)
            return false;

        return true;
    }

    private void Release(Resource resource)
    {
        resource.Released -= Release;
        _pool.Release(resource);
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