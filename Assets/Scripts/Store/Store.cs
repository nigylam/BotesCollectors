using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StoreFlagSpawner))]
public class Store : MonoBehaviour
{
    [SerializeField] private Transform _receptionPoint;
    [SerializeField] private Scanner _scanner;
    [SerializeField] private List<Transform> _unitPoints;
    [SerializeField] private TextCounter _resourcesCounter;
    [SerializeField] private int _unitCreateCost;
    [SerializeField] private int _storeCreateCost;

    private StoreFlagSpawner _flagSpawner;
    private UnitSpawner _unitSpawner;
    private ResourceDatabase _resourceDatabase;
    private List<Unit> _units = new();
    private Queue<Unit> _freeUnits = new();
    private int _resourcesCount = 0;

    private int ResourcesCount
    {
        get { return _resourcesCount; }
        set
        {
            _resourcesCount = value;

            if (_resourcesCount == UnitCreateCost)
            {
                if (TryCreateUnit())
                    _resourcesCount = 0;
            }

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

    private void Awake()
    {
        _flagSpawner = GetComponent<StoreFlagSpawner>();
    }

    private void Start()
    {
        ResourcesCount = 0;
    }

    private void Update()
    {
        _scanner.Scan();
    }

    private void OnEnable()
    {
        if (_resourceDatabase != null)
        {
            _scanner.ResourceFound += _resourceDatabase.AddResource;
            _resourceDatabase.ResourceAdded += CollectResource;
        }

        if (_units.Count == 0)
            return;

        foreach (var unit in _units)
            unit.TaskCompleted += OnTaskCompleted;
    }

    private void OnDisable()
    {
        _scanner.ResourceFound -= _resourceDatabase.AddResource;
        _resourceDatabase.ResourceAdded -= CollectResource;

        foreach (var unit in _units)
        {
            unit.TaskCompleted -= OnTaskCompleted;
        }
    }

    public void Initialize(int startUnitsCount, UnitSpawner unitSpawner, ResourceDatabase resourceDatabase, Button scanButton)
    {
        _scanner.Initialize(scanButton);
        _unitSpawner = unitSpawner;
        _resourceDatabase = resourceDatabase;
        _scanner.ResourceFound += _resourceDatabase.AddResource;
        _resourceDatabase.ResourceAdded += CollectResource;

        if (startUnitsCount > _unitPoints.Count)
            startUnitsCount = _unitPoints.Count;

        if (startUnitsCount == 0)
            startUnitsCount = 1;

        for (int i = 0; i < startUnitsCount; i++)
        {
            CreateUnit();
        }
    }

    public void OnClick()
    {
        Debug.Log("clicked");
    }

    private bool TryCreateUnit()
    {
        if (_unitPoints.Count > 0)
        {
            CreateUnit();
            return true;
        }

        return false;
    }

    private void CreateUnit()
    {
        Transform spawnPoint = _unitPoints[0];
        Unit unit = _unitSpawner.Spawn(spawnPoint, _receptionPoint);
        _unitPoints.RemoveAt(0);
        _units.Add(unit);
        _freeUnits.Enqueue(unit);
        unit.TaskCompleted += OnTaskCompleted;
    }

    private void CollectResource()
    {
        if (_freeUnits.TryDequeue(out Unit unit) == false)
            return;

        if (_resourceDatabase.TryGetResource(out Resource resource, this) == false)
        {
            _freeUnits.Enqueue(unit);
            return;
        }

        unit.SetResource(resource);
    }

    private void OnTaskCompleted(Resource resource, Unit unit)
    {
        _resourceDatabase.RemoveResource(resource);
        ResourcesCount++;
        AddFreeUnit(unit);
    }

    private void AddFreeUnit(Unit unit)
    {
        _freeUnits.Enqueue(unit);
        CollectResource();
    }
}
