using System;
using UnityEngine;

[RequireComponent(typeof(UnitMoving))]
public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _pickPoint;

    private UnitState _state = UnitState.Free;
    private float _speed;

    private Transform _storage;
    private Transform _waitPoint;
    private Vector3 _homePosition;
    private Resource _targetResource;
    private UnitMoving _moving;
    private UnitTraffic _traffic;

    public event Action<Unit> CompleteMission;

    public UnitState State => _state;

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

    public void Initialize(float speed, Transform resourceStorage, Transform waitPoint, UnitTraffic trafficLight)
    {
        _speed = speed;
        _storage = resourceStorage;
        _waitPoint = waitPoint;
        _traffic = trafficLight;
    }

    public void SetResource(Resource resource)
    {
        _targetResource = resource;
        ChangeTarget();
    }

    public void Move()
    {
        _moving.MoveToTarget(_speed);
    }

    public void GrantPass()
    {
        ChangeTarget();
    }

    private void ChangeTarget()
    {
        switch (_state)
        {
            case UnitState.Free:
                if (_targetResource == null)
                    return;

                _state = UnitState.GoingOut;
                _traffic.AskPass(this);
                break;
            case UnitState.GoingOut:
                _state = UnitState.GoingResource;
                _traffic.AcceptPass();
                _moving.SetTarget(_targetResource.transform.position, _state);
                break;
            case UnitState.GoingResource:
                _state = UnitState.GoingBase;
                _targetResource.Take(_pickPoint);
                _moving.SetTarget(_waitPoint.position, _state);
                break;
            case UnitState.GoingBase:
                _state = UnitState.GoingStorage;
                _traffic.AskPass(this);
                break;
            case UnitState.GoingStorage:
                _state = UnitState.GoingHome;
                _traffic.AcceptPass();
                _moving.SetTarget(_storage.position, _state);
                break;
            case UnitState.GoingHome:
                _state = UnitState.Free;
                _targetResource.Release();
                _targetResource = null;
                _moving.SetTarget(_homePosition, _state);
                CompleteMission?.Invoke(this);
                break;
        }
    }
}
