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
    private Transform _store;
    private Vector3 _homePosition;
    private Vector3 _flagPosition;
    private UnitTarget _currentTarget;
    private Dictionary<UnitTarget, Action> _states;

    public event Action<Resource, Unit> ResourceTaskCompleted;
    public event Action<Unit> ArrivedForBuilding;

    private void Awake()
    {
        _mover.Initialize(_speed);

        _states = new()
        {
            { UnitTarget.Home,  OnWentHome},
            { UnitTarget.Resource,  OnWentResource},
            { UnitTarget.Storage,  OnWentStorage},
            { UnitTarget.Flag, OnWentFlag},
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

    public void Initialize(Transform storePoint, Vector3 homePosition)
    {
        _store = storePoint;
        _homePosition = homePosition;
    }

    public void SetResource(Resource resource)
    {
        _targetResource = resource;
        SetTarget(UnitTarget.Resource);
    }

    public void SetBuildingTask(Vector3 position)
    {
        _flagPosition = position;
        SetTarget(UnitTarget.Flag);
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
            UnitTarget.Storage => _store.position,
            UnitTarget.Flag => _flagPosition,
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
        ResourceTaskCompleted?.Invoke(_targetResource, this);
    }

    private void OnWentFlag()
    {
        ArrivedForBuilding?.Invoke(this);
    }
}