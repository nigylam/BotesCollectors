using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitAi : MonoBehaviour
{
    private float _pickDistance = 1f;
    private float _distance;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void MoveToTarget(Transform target, float speed)
    {
        _distance = Vector3.Distance(transform.position, target.position);

        if( _distance < _pickDistance )
        {
            _agent.isStopped = true;
        }
        else
        {
            _agent.isStopped = false;
            _agent.destination = target.position;
            _agent.speed = speed;
        }
    }
}
