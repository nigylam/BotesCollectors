using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMoving : MonoBehaviour
{
    private float _closeDistance = 1f;
    private float _sqrDistance;
    private bool _isArrived = true;
    private Vector3 _target;
    private NavMeshAgent _agent;

    private float _baseCloseDistance = 1f;
    private float _resourceCloseDistance = 0.7f;
    private float _homeCloseDistance = 0.1f;

    public event Action Arrived;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Vector3 target, UnitState state)
    {
        switch (state)
        {
            case UnitState.GoingResource:
                _closeDistance = _resourceCloseDistance;
                break;
            case UnitState.GoingBase:
            case UnitState.GoingStorage:
            case UnitState.GoingHome:
                _closeDistance = _baseCloseDistance;
                break;
            default:
                _closeDistance = _homeCloseDistance;
                break;
        }

        _target = target;
        _isArrived = false;
        _agent.isStopped = false;
    }

    public void MoveToTarget(float speed)
    {
        if (_isArrived)
            return;

        _sqrDistance = Vector3.Distance(transform.position, _target);


        if (_sqrDistance < _closeDistance)
        {
            _agent.isStopped = true;
            _isArrived = true;
            Arrived?.Invoke();
        }
        else
        {
            _agent.destination = _target;
            _agent.speed = speed;
        }
    }
}
