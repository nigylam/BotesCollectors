using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _pickPoint;
    [SerializeField] private UnitMover _mover;
    [SerializeField] private float _speed;

    private Resource _targetResource;

    private Transform _storage;
    private Vector3 _homePosition;

    private UnitTarget _currentTarget;

    private Dictionary<UnitTarget, Action> _states;

    public event Action<Resource, Unit> TaskCompleted;

    private void Awake()
    {
        _homePosition = transform.position;
        _mover.Initialize(_speed);

        _states = new()
        {
            { UnitTarget.Home,  OnWentHome},
            { UnitTarget.Resource,  OnWentResource},
            { UnitTarget.Storage,  OnWentStorage},
        };
    }

    private void OnEnable()
    {
        _mover.Arrived += OnArrived;
    }

    private void OnDisable()
    {
        _mover.Arrived -= OnArrived;
    }

    public void Initialize(Transform storePoint)
    {
        _storage = storePoint;
    }

    public void SetResource(Resource resource)
    {
        _targetResource = resource;
        SetTarget(UnitTarget.Resource);
    }

    public void PauseMoving()
    {
        _mover.Pause();
    }

    public void ContinueMoving()
    {
        _mover.Continue();
    }

    private void OnArrived()
    {
        _states[_currentTarget]?.Invoke();
    }

    private void SetTarget(UnitTarget target)
    {
        _currentTarget = target;

        Vector3 position = target switch
        {
            UnitTarget.Home => _homePosition,
            UnitTarget.Resource => _targetResource.transform.position,
            UnitTarget.Storage => _storage.position,
            _ => transform.position
        };

        _mover.SetTarget(position, target);
    }

    private void OnWentHome() { }

    private void OnWentResource()
    {
        _targetResource.Take(_pickPoint);
        SetTarget(UnitTarget.Storage);
    }

    private void OnWentStorage()
    {
        _targetResource.Release();
        SetTarget(UnitTarget.Home);
        TaskCompleted?.Invoke(_targetResource, this);
    }
}