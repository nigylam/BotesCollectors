using UnityEngine;

public class Unit : MonoBehaviour
{
    private bool _isFree = true;
    private Transform _target;
    private float _speed;
    private UnitAi _unitAi;
    private Resource _targetResource;

    public bool IsFree => _isFree;

    private void Awake()
    {
        _unitAi = GetComponent<UnitAi>();
    }

    public void Initialize(float speed)
    {
        _speed = speed;
    }

    public void SetResource(Resource resource)
    {
        _isFree = false;
        _targetResource = resource;
        _target = resource.transform;
    }

    public void Move()
    {
        if (_isFree)
            return;

        _unitAi.MoveToTarget(_target, _speed);
    }
}
