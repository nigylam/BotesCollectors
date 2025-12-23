using System;
using UnityEngine;

[RequireComponent (typeof(Collider))]
public class StorePreview : MonoBehaviour, IColorable
{
    [SerializeField] private MeshRenderer[] _meshes;
    [SerializeField] private MouseFollower _follower;
    [SerializeField] private Color _availableBuildingColor;
    [SerializeField] private Color _notAvailableBuildingColor;

    private bool _isEnabled = true;
    private int _blockersCount = 0;

    public bool IsBuildingAvailable => _blockersCount == 0;

    private void Start()
    {
        ChangeColor(_availableBuildingColor);
    }

    private void Update()
    {
        if (_isEnabled == false)
            return;

        if (_follower.TryGetHitPoint<Plane>(out Vector3 point))
            ChangePosition(point);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsBlocking(other))
        {
            _blockersCount++;
            ChangeColor(_notAvailableBuildingColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsBlocking(other))
        {
            _blockersCount--;

            if (_blockersCount == 0)
                ChangeColor(_availableBuildingColor);
        }
    }

    private bool IsBlocking(Collider other)
    {
        return other.TryGetComponent<Resource>(out _)
            || other.TryGetComponent<Store>(out _)
            || other.TryGetComponent<Flag>(out _)
            || other.TryGetComponent<Unit>(out _);
    }

    public void ChangePosition(Vector3 position)
    {
        Vector3 correctedPosition = new Vector3(position.x, transform.position.y, position.z);
        transform.position = correctedPosition;
    }

    public void ChangeColor(Color color)
    {
        foreach (var mesh in _meshes)
            mesh.material.color = color;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        _isEnabled = true;
    }
}
