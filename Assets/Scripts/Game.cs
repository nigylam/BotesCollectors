using UnityEngine;

[RequireComponent(typeof(StoreSpawner))]
public class Game : MonoBehaviour
{
    [Header("Parametres")]
    [SerializeField] private float _resourceSpawnDelayMin;
    [SerializeField] private float _resourceSpawnDelayMax;
    [SerializeField] private float _resourceStartSpawnDelay;
    [SerializeField] private int _firstBaseStartUnitsCount;
    [SerializeField] private Vector3 _firstBaseSpawnPosition;

    [Header("Components links")]
    [SerializeField] private ResourceSpawner _resourceSpawner;

    private StoreSpawner _storeSpawner;
    private float _storeRadius = 2f;

    private void Awake()
    {
        _resourceSpawner.Initialize(_resourceSpawnDelayMin, _resourceSpawnDelayMax, _resourceStartSpawnDelay);
        _storeSpawner = GetComponent<StoreSpawner>();
    }

    private void Start()
    {
        _storeSpawner.Spawn(_firstBaseSpawnPosition, _firstBaseStartUnitsCount);
    }

    private void OnEnable()
    {
        _storeSpawner.StoreSpawned += OnStoreSpawned;
    }

    private void OnDisable()
    {
        _storeSpawner.StoreSpawned -= OnStoreSpawned;
    }

    private void OnStoreSpawned(Store store)
    {
        _resourceSpawner.ClearSpawnZones(store.transform.position, _storeRadius);
    }
}
