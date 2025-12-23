using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(FlagSpawner))]
[RequireComponent(typeof(StoreSpawner))]
[RequireComponent(typeof(ScreenClicker))]
public class StoreBuilder : MonoBehaviour
{
    [SerializeField] private Transform _parent;

    private ScreenClicker _screenClicker;
    private StoreSpawner _storeSpawner;
    private Store _chosenStore;
    private List<Store> _spawnedStores = new();
    private FlagSpawner _flagSpawner;
    private bool _isPlacingFlagActive = false;

    private void Awake()
    {
        _screenClicker = GetComponent<ScreenClicker>();
        _storeSpawner = GetComponent<StoreSpawner>();
        _flagSpawner = GetComponent<FlagSpawner>();
    }

    private void OnEnable()
    {
        _screenClicker.StoreClicked += OnStoreClick;
        _screenClicker.PlaneClicked += OnPlaneClick;
        _storeSpawner.StoreSpawned += AddNewStore;

        if(_spawnedStores.Count > 0)
        {
            foreach (Store store in _spawnedStores)
            {
                store.BuildingUnitArrived += OnBuildingUnitArrived;
                store.FlagReleased += ReleaseFlag;
            }
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
            {
                store.BuildingUnitArrived -= OnBuildingUnitArrived;
                store.FlagReleased -= ReleaseFlag;
            }
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
        var flag = _flagSpawner.Spawn(position, _parent);
        _chosenStore.SetFlag(flag);
    }

    private void AddNewStore(Store store)
    {
        _spawnedStores.Add(store);
        store.BuildingUnitArrived += OnBuildingUnitArrived;
        store.FlagReleased += ReleaseFlag;
    }

    private void OnBuildingUnitArrived(Store store, Unit unit)
    {
        _storeSpawner.Spawn(store.BuildLocalPosition, 0, unit);
    }

    private void ReleaseFlag(Flag flag)
    {
        _flagSpawner.Release(flag);
    }
}
