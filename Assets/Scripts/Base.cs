using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Scanner _scanner;
    [SerializeField] private List<Unit> _units;
    [SerializeField] private Transform _resourceStorage;

    private Queue<Unit> _freeUnits;
    private List<Resource> _resourcesToCollect = new();

    private void Awake()
    {
        _freeUnits = new Queue<Unit>(_units);
    }

    private void OnEnable()
    {
        _scanner.ResourceSelected += AddResource;

        foreach (var unit in _units)
            unit.CompleteMission += AddFreeUnit;
    }

    private void OnDisable()
    {
        _scanner.ResourceSelected -= AddResource;

        foreach (var unit in _units)
            unit.CompleteMission -= AddFreeUnit;
    }

    public void Initialize(float unitSpeed)
    {
        foreach (var unit in _units)
            unit.Initialize(unitSpeed, _resourceStorage);
    }

    public void UnitMoving()
    {
        foreach (var unit in _units)
            unit.Move();
    }

    public void CollectResource()
    {
        if (_resourcesToCollect.Count == 0 || _freeUnits.TryDequeue(out Unit unit) == false)
            return;

        Resource resource = GetNearestResource();
        _resourcesToCollect.Remove(resource);

        unit.SetResource(resource);
    }

    private void AddFreeUnit(Unit unit)
    {
        _freeUnits.Enqueue(unit);
        CollectResource();
    }

    private void AddResource(Resource resource)
    {
        if (_resourcesToCollect.Contains(resource) || resource.IsSelected)
            return;

        resource.Select();
        _resourcesToCollect.Add(resource);
        CollectResource();
    }

    private Resource GetNearestResource()
    {
        if (_resourcesToCollect.Count == 0)
            return null;

        if (_resourcesToCollect.Count == 1)
            return _resourcesToCollect[0];

        Resource nearestResource = _resourcesToCollect[0];

        foreach (Resource resource in _resourcesToCollect)
        {
            if (Vector3.SqrMagnitude(resource.transform.position - transform.position) < Vector3.SqrMagnitude(nearestResource.transform.position - transform.position))
                nearestResource = resource;
        }

        return nearestResource;
    }
}
