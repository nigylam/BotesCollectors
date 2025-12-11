using System;
using UnityEngine;

[RequireComponent(typeof(UnitMoving))]
public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _pickPoint;

    private float _speed;

    private Resource _targetResource;
    private UnitMoving _moving;
    private Transform _storage;
    private Transform _enter;
    private Transform _exit;
    private Vector3 _homePosition;
    private UnitTarget _target = UnitTarget.Home;

    public event Action TaskCompleted;
    public event Action<Unit> Freed;

    private void Awake()
    {
        _moving = GetComponent<UnitMoving>();
        _homePosition = transform.position;
    }

    private void OnEnable()
    {
        _moving.Arrived += ChangeTarget;
    }

    private void OnDisable()
    {
        _moving.Arrived -= ChangeTarget;
    }

    public void Initialize(float speed, Transform resourceStorage, Transform enter, Transform exit)
    {
        _speed = speed;
        _storage = resourceStorage;
        _enter = enter;
        _exit = exit;
    }

    public void Move()
    {
        _moving.MoveToTarget(_speed);
    }

    public void SetResource(Resource resource)
    {
        _targetResource = resource;
        ChangeTarget();
    }

    public void PauseMoving()
    {
        _moving.Pause();
    }

    public void ContinueMoving()
    {
        _moving.Continue();
    }

    private void ChangeTarget()
    {
        switch (_target)
        {
            case UnitTarget.Home:
                if (_targetResource == null)
                    return;

                _target = UnitTarget.Resource;
                _moving.SetTarget(_targetResource.transform.position, _target);
                break;
            case UnitTarget.Resource:
                _target = UnitTarget.Enter;
                _targetResource.Take(_pickPoint);
                _moving.SetTarget(_enter.position, _target);
                break;
            case UnitTarget.Enter:
                _target = UnitTarget.Storage;
                _moving.SetTarget(_storage.position, _target);
                break;
            case UnitTarget.Storage:
                _target = UnitTarget.Exit;
                _targetResource.Release();
                _targetResource = null;
                TaskCompleted?.Invoke();
                _moving.SetTarget(_exit.position, _target);
                break;
            case UnitTarget.Exit:
                _target = UnitTarget.Home;
                _moving.SetTarget(_homePosition, _target);
                Freed?.Invoke(this);
                break;
        }
    }
}
