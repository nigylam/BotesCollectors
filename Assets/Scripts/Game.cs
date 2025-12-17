using UnityEngine;

[RequireComponent(typeof(StoreSpawner))]
public class Game : MonoBehaviour
{
    [Header("Parametres")]
    [SerializeField] private float _resourceSpawnDelayMin;
    [SerializeField] private float _resourceSpawnDelayMax;
    [SerializeField] private float _resourceStartSpawnDelay;
    [SerializeField] private Vector3 _firstBaseSpawnPosition;
    [SerializeField] private int _firstBaseStartUnitsCount;

    [Header("Components links")]
    [SerializeField] private ResourceSpawner _resourceSpawner;

    private StoreSpawner _storeSpawner;

    private void Awake()
    {
        _resourceSpawner.Initialize(_resourceSpawnDelayMin, _resourceSpawnDelayMax, _resourceStartSpawnDelay);
        _storeSpawner = GetComponent<StoreSpawner>();
    }

    private void Start()
    {
        _storeSpawner.Spawn(_firstBaseSpawnPosition, _firstBaseStartUnitsCount);
    }
}
