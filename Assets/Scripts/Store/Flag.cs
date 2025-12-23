using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour, IColorable
{
    [SerializeField] private MeshRenderer _flagMesh;
    [SerializeField] private MeshRenderer _flagPoleMesh;

    public void ChangeColor(Color color)
    {
        _flagMesh.material.color = color;
        _flagPoleMesh.material.color = color;
    }
}
