using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMoving : MonoBehaviour
{
    private float _closeDistance = 1f;
    private float _distance;
    private bool _isArrived = true;
    private Vector3 _target;
    private NavMeshAgent _agent;
    private float _baseDistance = 1f;
    private float _resourceDistance = 0.7f;
    private float _homeDistance = 0.1f;

    public event Action Arrived;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Vector3 target, UnitState state)
    {
        switch (state)
        {
            case UnitState.GoingToResource:
                _closeDistance = _resourceDistance; 
                break;
            case UnitState.GoingToBase:
                _closeDistance = _baseDistance;
                break;
            default:
                _closeDistance = _homeDistance; 
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

        _distance = Vector3.Distance(transform.position, _target);

        if( _distance < _closeDistance )
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
