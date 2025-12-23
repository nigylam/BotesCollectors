using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreUnitCommander : MonoBehaviour
{
    [SerializeField] private List<Transform> _unitPoints;
    [SerializeField] private Transform _receptionPoint;

    private UnitSpawner _unitSpawner;
    private List<Unit> _units = new();
    private Queue<Unit> _freeUnits = new();
    private Unit _unitBuilder;
    private Color _color;

    public int UnitsCount => _units.Count;

    public event Action<Resource, Unit> TaskCompleted;

    private void OnEnable()
    {
        if (_units.Count == 0)
            return;

        foreach (var unit in _units)
            unit.TaskCompleted += OnTaskCompleted;
    }

    private void OnDisable()
    {
        foreach (var unit in _units)
            unit.TaskCompleted -= OnTaskCompleted;
    }

    public void Initialize(UnitSpawner unitSpawner, int startUnitsCount, Unit startUnit, Color color)
    {
        _unitSpawner = unitSpawner;
        _color = color;

        if (startUnitsCount > _unitPoints.Count)
            startUnitsCount = _unitPoints.Count;

        if (startUnitsCount == 0 && startUnit == null)
            startUnitsCount = 1;

        if (startUnit != null)
        {
            CreateUnit(startUnit);
            return;
        }

        for (int i = 0; i < startUnitsCount; i++)
            CreateUnit();
    }

    public void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
        unit.TaskCompleted -= OnTaskCompleted;
    }

    public bool TryCreateUnit()
    {
        if (_unitPoints.Count > 0)
        {
            CreateUnit();
            return true;
        }

        return false;
    }

    public void AddBuildTask(Vector3 buildPosition)
    {
        _unitBuilder = _freeUnits.Dequeue();
        _unitBuilder.SetBuildingTask(buildPosition);
    }

    public void AddResourceTask(Resource resource)
    {
        _freeUnits.Dequeue().SetResourceTask(resource);
    }

    public bool HaveFreeUnits() => _freeUnits.Count > 0;

    public void ChangeFlagPosition(Vector3 newPosition)
    {
        if (_unitBuilder != null) 
        {
            _unitBuilder.SetBuildingTask(newPosition);
        }
    }

    private void CreateUnit()
    {
        Transform spawnPoint = _unitPoints[0];
        Unit unit = _unitSpawner.Spawn(spawnPoint, _receptionPoint);
        RegisterUnit(unit);
    }

    private void CreateUnit(Unit unit)
    {
        Transform spawnPoint = _unitPoints[0];
        unit.Initialize(_receptionPoint, spawnPoint.position);
        RegisterUnit(unit);
    }

    private void RegisterUnit(Unit unit)
    {
        unit.ChangeColor(_color);
        _unitPoints.RemoveAt(0);
        _units.Add(unit);
        _freeUnits.Enqueue(unit);
        unit.TaskCompleted += OnTaskCompleted;
    }

    private void OnTaskCompleted(Resource resource, Unit unit)
    {
        if (resource != null)
            _freeUnits.Enqueue(unit);
        else
            _unitBuilder = null;

        TaskCompleted?.Invoke(resource, unit);
    }
}
