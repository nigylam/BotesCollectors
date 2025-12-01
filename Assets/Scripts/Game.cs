using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Parametres")]
    [SerializeField] private float _resourceSpawnDelayMin;
    [SerializeField] private float _resourceSpawnDelayMax;
    [SerializeField] private float _resourceStartSpawnDelay;

    [Header("Components links")]
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private Scanner _scanner;

    private void Awake()
    {
        _resourceSpawner.Initialize(_resourceSpawnDelayMin, _resourceSpawnDelayMax, _resourceStartSpawnDelay);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        _scanner.Scan();
    }
}
