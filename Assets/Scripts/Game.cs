using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private float _resourceSpawnDelayMin;
    [SerializeField] private float _resourceSpawnDelayMax;
    [SerializeField] private float _resourceStartSpawnDelay;

    [SerializeField] private ResourceSpawner _resourceSpawner;

    private void Awake()
    {
        _resourceSpawner.Initialize(_resourceSpawnDelayMin, _resourceSpawnDelayMax, _resourceStartSpawnDelay);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
