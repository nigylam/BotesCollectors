using System;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private Material _outlineMaterial;

    private bool _isSelected = false;
    private MeshRenderer _meshRenderer;
    private Material _baseMaterial;

    public event Action Took;

    public bool IsSelected => _isSelected;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _baseMaterial = _meshRenderer.material;
    }

    public void Take(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        Took?.Invoke();
    }

    public void Select()
    {
        if (_isSelected)
            return;

        _isSelected = true;
        SetMaterials();
    }

    private void SetMaterials()
    {
        List<Material> materials = new() { _baseMaterial, _outlineMaterial};
        _meshRenderer.SetMaterials(materials);
    }
}
