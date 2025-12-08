using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class Resource : MonoBehaviour
{
    [SerializeField] private Material _outlineMaterial;

    private bool _isSelected = false;
    private MeshRenderer _meshRenderer;
    private Material _baseMaterial;
    private NavMeshObstacle _navmesh;

    public event Action Took;
    public event Action<Resource> Released;

    public bool IsSelected => _isSelected;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _baseMaterial = _meshRenderer.material;
        _navmesh = GetComponent<NavMeshObstacle>();
    }

    private void OnEnable()
    {
        _navmesh.enabled = true;
    }

    public void Take(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        _navmesh.enabled = false;
        Took?.Invoke();
    }

    public void Select()
    {
        if (_isSelected)
            return;

        _isSelected = true;
        SetMaterials();
    }

    public void Release()
    {
        _meshRenderer.SetMaterials(new() { _baseMaterial });
        _isSelected = false;
        Released?.Invoke(this);
    }

    private void SetMaterials()
    {
        List<Material> materials = new() { _baseMaterial, _outlineMaterial};
        _meshRenderer.SetMaterials(materials);
    }
}
