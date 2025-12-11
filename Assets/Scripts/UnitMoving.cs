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

    private float _homeCloseDistance = 0.1f;
    private float _resourceCloseDistance = 0.7f;
    private float _storageCloseDistance = 1f;

    public event Action Arrived;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Vector3 target, UnitTarget targetType)
    {
        switch (targetType)
        {
            case UnitTarget.Home:
                _closeDistance = _homeCloseDistance;
                break;
            case UnitTarget.Resource:
                _closeDistance = _resourceCloseDistance;
                break;
            case UnitTarget.Storage:
            case UnitTarget.Enter:
            case UnitTarget.Exit:
                _closeDistance = _storageCloseDistance;
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

        _sqrDistance = Vector3.SqrMagnitude(transform.position - _target);


        if (_sqrDistance < _closeDistance * _closeDistance)
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

    public void Pause()
    {
        _agent.isStopped = true;
    }

    public void Continue()
    {
        _agent.isStopped = false;
    }
}
