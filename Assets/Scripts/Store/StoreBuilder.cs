using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StoreSpawner))]
[RequireComponent(typeof(ScreenClicker))]
public class StoreBuilder : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private Flag _flagPrefab;

    private ScreenClicker _screenClicker;
    private StoreSpawner _storeSpawner;
    private bool _isPlacingFlagActive = false;
    private Store _chosenStore;
    private List<Store> _spawnedStores = new();

    private void Awake()
    {
        _screenClicker = GetComponent<ScreenClicker>();
        _storeSpawner = GetComponent<StoreSpawner>();
    }

    private void OnEnable()
    {
        _screenClicker.StoreClicked += OnStoreClick;
        _screenClicker.PlaneClicked += OnPlaneClick;
        _storeSpawner.StoreSpawned += AddNewStore;

        if(_spawnedStores.Count > 0)
        {
            foreach (Store store in _spawnedStores)
                store.BuildingUnitArrived += OnBuildingUnitArrived;
        }
    }

    private void OnDisable()
    {
        _screenClicker.StoreClicked -= OnStoreClick;
        _screenClicker.PlaneClicked -= OnPlaneClick;
        _storeSpawner.StoreSpawned -= AddNewStore;

        if (_spawnedStores.Count > 0)
        {
            foreach (Store store in _spawnedStores)
                store.BuildingUnitArrived -= OnBuildingUnitArrived;
        }
    }

    private void OnStoreClick(Store store)
    {
        if(store.CanBuildNewBase == false) 
            return;

        if (_isPlacingFlagActive)
            return;

        _isPlacingFlagActive = true;
        _chosenStore = store;
        store.ActivateBuildingMark();
    }

    private void OnPlaneClick(Vector3 position)
    {
        if(_isPlacingFlagActive == false)
            return;

        _isPlacingFlagActive = false;
        Vector3 flagPosition = new Vector3(position.x, _flagPrefab.transform.position.y, position.z);
        var flag = Instantiate(_flagPrefab, flagPosition, Quaternion.identity, _parent); ;
        _chosenStore.SetFlag(flag);
    }

    private void AddNewStore(Store store)
    {
        _spawnedStores.Add(store);
        store.BuildingUnitArrived += OnBuildingUnitArrived;
    }

    private void OnBuildingUnitArrived(Store store, Unit unit)
    {
        _storeSpawner.Spawn(store.BuildLocalPosition, 0, unit);
    }
}
