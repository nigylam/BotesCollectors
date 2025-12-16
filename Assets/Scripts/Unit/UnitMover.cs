using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMover : MonoBehaviour
{
    private float _closeDistance = 1f;
    private Vector3 _target;
    private NavMeshAgent _agent;
    private Coroutine _moveToTarget;

    private float _homeCloseDistance = 0.1f;
    private float _resourceCloseDistance = 0.7f;
    private float _storageCloseDistance = 1f;

    public event Action Arrived;

    public void Initialize(float speed)
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;
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
                _closeDistance = _storageCloseDistance;
                break;
        }

        _target = target;
        _agent.destination = _target;

        if (_moveToTarget != null)
            StopCoroutine(_moveToTarget);

        _moveToTarget = StartCoroutine(MoveToTarget());
    }

    public void Pause()
    {
        _agent.isStopped = true;
    }

    public void Continue()
    {
        _agent.isStopped = false;
    }

    private IEnumerator MoveToTarget()
    {
        while (IsEnoughClose() == false)
        {
            yield return null;
        }

        Arrived?.Invoke();
    }

    private bool IsEnoughClose()
    {
        return Vector3.SqrMagnitude(transform.position - _target) <= _closeDistance * _closeDistance;
    }
}
