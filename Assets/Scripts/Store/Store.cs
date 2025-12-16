using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField] private Transform _receptionPoint;
    [SerializeField] private Scanner _scanner;
    [SerializeField] private List<Transform> _unitPoints;
    [SerializeField] private TextCounter _resourcesCounter;

    private UnitSpawner _unitSpawner;
    private ResourceDatabase _resourceDatabase;
    private List<Unit> _units = new();
    private Queue<Unit> _freeUnits;
    private int _resourcesCount = 0;

    private void Start()
    {
        _resourcesCounter.Change(_resourcesCount);
    }

    private void Update()
    {
        _scanner.Scan();
    }

    private void OnEnable()
    {
        if (_resourceDatabase != null)
        {
            _scanner.ResourceFound += _resourceDatabase.AddResource;
            _resourceDatabase.ResourceAdded += CollectResource;
        }

        if (_units.Count == 0)
            return;

        foreach (var unit in _units)
            unit.TaskCompleted += OnTaskCompleted;
    }

    private void OnDisable()
    {
        _scanner.ResourceFound -= _resourceDatabase.AddResource;
        _resourceDatabase.ResourceAdded -= CollectResource;

        foreach (var unit in _units)
        {
            unit.TaskCompleted -= OnTaskCompleted;
        }
    }

    public void Initialize(UnitSpawner unitSpawner, ResourceDatabase resourceDatabase, Button scanButton)
    {
        _scanner.Initialize(scanButton);
        _unitSpawner = unitSpawner;
        _resourceDatabase = resourceDatabase;

        _scanner.ResourceFound += _resourceDatabase.AddResource;
        _resourceDatabase.ResourceAdded += CollectResource;

        foreach(var unitPoint in _unitPoints)
        {
            Unit unit = _unitSpawner.Spawn(unitPoint, _receptionPoint);
            _units.Add(unit);
            unit.TaskCompleted += OnTaskCompleted;
        }

        _freeUnits = new Queue<Unit>(_units);
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

    private void OnTaskCompleted(Resource resource, Unit unit)
    {
        _resourceDatabase.RemoveResource(resource);
        _resourcesCounter.Change(++_resourcesCount);
        AddFreeUnit(unit);
    }

    private void AddFreeUnit(Unit unit)
    {
        _freeUnits.Enqueue(unit);
        CollectResource();
    }
}
