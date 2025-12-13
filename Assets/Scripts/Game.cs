using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Parametres")]
    [SerializeField] private float _resourceSpawnDelayMin;
    [SerializeField] private float _resourceSpawnDelayMax;
    [SerializeField] private float _resourceStartSpawnDelay;
    [SerializeField] private float _unitSpeed;

    [Header("Components links")]
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private Scanner _scanner;
    [SerializeField] private Base _base;
    [SerializeField] private TextCounter _resourcesCounter;

    private void Awake()
    {
        _resourceSpawner.Initialize(_resourceSpawnDelayMin, _resourceSpawnDelayMax, _resourceStartSpawnDelay);
        _base.Initialize(_unitSpeed);
    }

    private void OnEnable()
    {
        _base.ResourcesCountChanged += _resourcesCounter.Change;
    }

    private void OnDisable()
    {
        _base.ResourcesCountChanged -= _resourcesCounter.Change;
    }
}
