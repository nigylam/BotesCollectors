using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private List<Unit> _units;
    [SerializeField] private Transform _resourceStorage;
    [SerializeField] private Transform _enter;
    [SerializeField] private Transform _exit;
    [SerializeField] private Scanner _scanner;
    [SerializeField] private ResourceDatabase _resourceDatabase;

    private Queue<Unit> _freeUnits;
    private int _resourcesCount = 0;

    public event Action<int> ResourcesCountChanged;

    private void Awake()
    {
        _freeUnits = new Queue<Unit>(_units);
    }

    private void Start()
    {
        ResourcesCountChanged?.Invoke(_resourcesCount);
    }

    private void Update()
    {
        _scanner.Scan();
    }

    private void OnEnable()
    {
        _scanner.ResourceFound += _resourceDatabase.AddResource;
        _resourceDatabase.ResourceAdded += CollectResource;

        foreach (var unit in _units)
        {
            unit.TaskCompleted += ProcessResource;
            unit.Freed += AddFreeUnit;
        }
    }

    private void OnDisable()
    {
        _scanner.ResourceFound -= _resourceDatabase.AddResource;

        foreach (var unit in _units)
        {
            unit.TaskCompleted -= ProcessResource;
            unit.Freed -= AddFreeUnit;
        }
    }

    public void Initialize(float unitSpeed)
    {
        foreach (var unit in _units)
            unit.Initialize(unitSpeed, _resourceStorage, _enter, _exit);
    }

    public void CollectResource()
    {
        if (_freeUnits.TryDequeue(out Unit unit) == false)
            return;

        if(_resourceDatabase.TryGetResource(out Resource resource, this) == false)
        {
            _freeUnits.Enqueue(unit);
            return;
        }

        unit.SetResource(resource);
    }

    private void ProcessResource(Resource resource)
    {
        _resourceDatabase.RemoveResource(resource);
        ResourcesCountChanged?.Invoke(++_resourcesCount);
    }

    private void AddFreeUnit(Unit unit)
    {
        _freeUnits.Enqueue(unit);
        CollectResource();
    }
}
