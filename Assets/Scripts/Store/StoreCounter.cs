using System;
using UnityEngine;

public class StoreCounter : MonoBehaviour
{
    [SerializeField] private int _unitCreateCost;
    [SerializeField] private int _storeCreateCost;
    [SerializeField] private TextCounter _resourcesCounter;

    private int _resourcesCount = 0;
    private bool _isStoreCreatingPriority = false;

    public event Action StoreAmountReached;
    public event Action UnitAmountReached;

    public bool IsEnoughForStore() => ResourcesCount >= StoreCreateCost;

    private void Start()
    {
        ResourcesCount = 0;
    }

    private int ResourcesCount
    {
        get { return _resourcesCount; }
        set
        {
            _resourcesCount = value;

            if (_isStoreCreatingPriority)
            {
                if (_resourcesCount >= StoreCreateCost)
                {
                    StoreAmountReached?.Invoke();
                }
            }
            else if (_resourcesCount >= UnitCreateCost)
            {
                UnitAmountReached?.Invoke();
            }

            if (_resourcesCount < 0)
                _resourcesCount = 0;

            _resourcesCounter.Change(_resourcesCount);
        }
    }

    private int UnitCreateCost
    {
        get { return _unitCreateCost; }
        set
        {
            _unitCreateCost = value;

            if (value < 1)
                _unitCreateCost = 1;
        }
    }

    private int StoreCreateCost
    {
        get { return _storeCreateCost; }
        set
        {
            _storeCreateCost = value;

            if (value < 1)
                _storeCreateCost = 1;
        }
    }

    public void SetStoreCreatingPriority()
    {
        _isStoreCreatingPriority = true;
        ResourcesCount = ResourcesCount;
    }

    public void SetUnitCreatingPriority()
    {
        _isStoreCreatingPriority = false;
        ResourcesCount = ResourcesCount;
    }

    public void IncreaseResources() => ResourcesCount++;

    public void SpendUnitCost() => ResourcesCount -= UnitCreateCost;

    public void SpendStoreCost() => ResourcesCount -= StoreCreateCost;
}
