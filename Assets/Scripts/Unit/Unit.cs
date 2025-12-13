using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _pickPoint;
    [SerializeField] private UnitMover _mover;

    private Resource _targetResource;

    private Transform _storage;
    private Transform _enter;
    private Transform _exit;
    private Vector3 _homePosition;

    private UnitTarget _currentTarget;

    private Dictionary<UnitTarget, Action> _states;

    public event Action<Resource> TaskCompleted;
    public event Action<Unit> Freed;

    private void Awake()
    {
        _homePosition = transform.position;

        _states = new()
        {
            { UnitTarget.Home,  OnWentHome},
            { UnitTarget.Resource,  OnWentResource},
            { UnitTarget.Enter,  OnWentEnter},
            { UnitTarget.Storage,  OnWentStorage},
            { UnitTarget.Exit,  OnWentExit}
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

    public void Initialize(float speed, Transform resourceStorage, Transform enter, Transform exit)
    {
        _mover.Initialize(speed);
        _storage = resourceStorage;
        _enter = enter;
        _exit = exit;
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
            UnitTarget.Enter => _enter.position,
            UnitTarget.Storage => _storage.position,
            UnitTarget.Exit => _exit.position,
            _ => transform.position
        };

        _mover.SetTarget(position, target);
    }

    private void OnWentHome() { }

    private void OnWentResource()
    {
        _targetResource.Take(_pickPoint);
        SetTarget(UnitTarget.Enter);
    }

    private void OnWentEnter()
    {
        SetTarget(UnitTarget.Storage);
    }

    private void OnWentStorage()
    {
        _targetResource.Release();
        TaskCompleted?.Invoke(_targetResource);
        _targetResource = null;
        SetTarget(UnitTarget.Exit);
    }

    private void OnWentExit()
    {
        SetTarget(UnitTarget.Home);
        Freed?.Invoke(this);
    }
}