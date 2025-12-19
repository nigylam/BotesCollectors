using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(UnitSpawner))]
[RequireComponent (typeof(ResourceDatabase))]
public class StoreSpawner : MonoBehaviour
{
    [SerializeField] private Store _storePrefab;
    [SerializeField] private NavMeshSurface _meshSurface;
    [SerializeField] private Button _scanButton;
    [SerializeField] private Transform _parrent;

    private UnitSpawner _unitSpawner;
    private ResourceDatabase _resourceDatabase;

    public event Action<Store> StoreSpawned;

    private void Awake()
    {
        _unitSpawner = GetComponent<UnitSpawner>();
        _resourceDatabase = GetComponent<ResourceDatabase>();
    }

    public Store Spawn(Vector3 spawnLocalPosition, int startUnitsCount, Unit startUnit = null)
    {
        Vector3 position = new Vector3(spawnLocalPosition.x, _storePrefab.transform.position.y, spawnLocalPosition.z);

        Store store = Instantiate(_storePrefab, _parrent);
        store.transform.localPosition = position;
        store.Initialize(startUnitsCount, _unitSpawner, _resourceDatabase, _scanButton, startUnit);
        _meshSurface.BuildNavMesh();
        StoreSpawned?.Invoke(store);
        return store;
    }
}
