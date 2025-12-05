using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _pickPoint;

    private UnitState _state;
    private float _speed;

    private Transform _resourceStorage;
    private Vector3 _homePosition;
    private UnitMoving _moving;
    private Resource _targetResource;

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

    public void Initialize(float speed, Transform resourceStorage)
    {
        _speed = speed;
        _resourceStorage = resourceStorage;
    }

    public void SetResource(Resource resource)
    {
        _state = UnitState.GoingToResource;
        _targetResource = resource;
        _moving.SetTarget(resource.transform.position, _state);
    }

    private void ChangeTarget()
    {
        if (_state == UnitState.GoingToResource)
        {
            _state = UnitState.GoingToBase;
            _targetResource.Take(_pickPoint);
            _moving.SetTarget(_resourceStorage.position, _state);
        }
        else if (_state == UnitState.GoingToBase)
        {
            _state = UnitState.GoingToHomePosition;
            _targetResource.transform.SetParent(_resourceStorage);
            Destroy(_targetResource.gameObject);
            _moving.SetTarget(_homePosition, _state);
        }
        else if (_state == UnitState.GoingToHomePosition)
        {
            _targetResource = null;
            _state = UnitState.Free;
            CompleteMission?.Invoke(this);
        }
    }

    public void Move()
    {
        if (_state == UnitState.Free)
            return;

        _moving.MoveToTarget(_speed);
    }
}
