using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField] private Transform _receptionPoint;
    [SerializeField] private Scanner _scanner;
    [SerializeField] private List<Transform> _unitPoints;
    [SerializeField] private TextCounter _resourcesCounter;
    [SerializeField] private RectTransform _buildingMark;
    [SerializeField] private int _unitCreateCost;
    [SerializeField] private int _storeBuildCost;

    private UnitSpawner _unitSpawner;
    private ResourceDatabase _resourceDatabase;
    private List<Unit> _units = new();
    private Queue<Unit> _freeUnits = new();
    private Flag _buildingFlag;
    private int _resourcesCount = 0;
    private bool _isReadyForBuilding = false;
    private bool _isBuildPriority = false;

    public event Action<Store, Unit> BuildingUnitArrived;

    public Vector3 BuildLocalPosition => _buildingFlag.transform.localPosition;

    private int ResourcesCount
    {
        get { return _resourcesCount; }
        set
        {
            _resourcesCount = value;

            if (_isBuildPriority)
            {
                if (_resourcesCount >= StoreBuildCost)
                {
                    _resourcesCount = 0;
                    AddBuildOperation();
                }
            }
            else if (_resourcesCount >= UnitCreateCost)
            {
                if (TryAddUnit())
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

    private int StoreBuildCost
    {
        get { return _storeBuildCost; }
        set
        {
            _storeBuildCost = value;

            if (value < 1)
                _storeBuildCost = 1;
        }
    }

    private void Start()
    {
        ResourcesCount = 0;
        _buildingMark.gameObject.SetActive(false);
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
            _resourceDatabase.ResourceAdded += SetNewTask;
        }

        if (_units.Count == 0)
            return;

        foreach (var unit in _units)
        {
            unit.ResourceTaskCompleted += OnResourceCollectTaskCompleted;
            unit.ArrivedForBuilding += OnUnitArrivedBuilding;
        }
    }

    private void OnDisable()
    {
        _scanner.ResourceFound -= _resourceDatabase.AddResource;
        _resourceDatabase.ResourceAdded -= SetNewTask;

        foreach (var unit in _units)
        {
            unit.ResourceTaskCompleted -= OnResourceCollectTaskCompleted;
            unit.ArrivedForBuilding -= OnUnitArrivedBuilding;
        }
    }

    public void Initialize(int startUnitsCount, UnitSpawner unitSpawner, ResourceDatabase resourceDatabase, Button scanButton, Unit startUnit = null)
    {
        _scanner.Initialize(scanButton);
        _unitSpawner = unitSpawner;
        _resourceDatabase = resourceDatabase;
        _scanner.ResourceFound += _resourceDatabase.AddResource;
        _resourceDatabase.ResourceAdded += SetNewTask;

        if (startUnitsCount > _unitPoints.Count)
            startUnitsCount = _unitPoints.Count;

        if (startUnitsCount == 0 && startUnit == null)
            startUnitsCount = 1;

        if (startUnit != null)
        {
            AddUnit(startUnit);
            return;
        }

        for (int i = 0; i < startUnitsCount; i++)
            AddUnit();
    }

    public void ActivateBuildingMark()
    {
        _buildingMark.gameObject.SetActive(true);
    }

    public void SetFlag(Flag flag)
    {
        if(_buildingFlag != null)
        {
            Destroy(_buildingFlag.gameObject);
        }

        _buildingFlag = flag;
        _isBuildPriority = true;
    }

    private bool TryAddUnit()
    {
        if (_unitPoints.Count > 0)
        {
            AddUnit();
            return true;
        }

        return false;
    }

    private void AddUnit()
    {
        Transform spawnPoint = _unitPoints[0];
        Unit unit = _unitSpawner.Spawn(spawnPoint, _receptionPoint);
        RegisterUnit(unit);
    }

    private void AddUnit(Unit unit)
    {
        Transform spawnPoint = _unitPoints[0];
        unit.Initialize(_receptionPoint, spawnPoint.position);
        RegisterUnit(unit);
    }

    private void RegisterUnit(Unit unit)
    {
        _unitPoints.RemoveAt(0);
        _units.Add(unit);
        _freeUnits.Enqueue(unit);
        unit.ResourceTaskCompleted += OnResourceCollectTaskCompleted;
        unit.ArrivedForBuilding += OnUnitArrivedBuilding;
    }

    private void AddBuildOperation()
    {
        _isBuildPriority = false;
        _isReadyForBuilding = true;
    }

    private void SetNewTask()
    {
        if (_freeUnits.TryDequeue(out Unit unit) == false)
            return;

        if (_isReadyForBuilding)
        {
            unit.SetBuildingTask(_buildingFlag.transform.position);
            _isReadyForBuilding = false;
            return;
        }
        
        if (_resourceDatabase.TryGetResource(out Resource resource, this) == false)
        {
            _freeUnits.Enqueue(unit);
            return;
        }

        unit.SetResource(resource);
    }

    private void OnResourceCollectTaskCompleted(Resource resource, Unit unit)
    {
        _resourceDatabase.RemoveResource(resource);
        ResourcesCount++;
        AddFreeUnit(unit);
    }

    private void OnUnitArrivedBuilding(Unit unit)
    {
        BuildingUnitArrived?.Invoke(this, unit);
        RemoveUnit(unit);
        Destroy(_buildingFlag.gameObject);
        _buildingFlag = null;
        _buildingMark.gameObject.SetActive(false);
    }

    private void AddFreeUnit(Unit unit)
    {
        _freeUnits.Enqueue(unit);
        SetNewTask();
    }

    private void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
        unit.ArrivedForBuilding -= OnUnitArrivedBuilding;
        unit.ResourceTaskCompleted -= OnResourceCollectTaskCompleted;
    }
}
