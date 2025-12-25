using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StoreUnitCommander))]
public class Store : MonoBehaviour, IColorable
{
    [SerializeField] private Scanner _scanner;
    [SerializeField] private RectTransform _mark;
    [SerializeField] private int _minUnitsCountForBuilding;
    [SerializeField] private StoreCounter _counter;

    private Color _color;
    private StoreUnitCommander _unitCommander;
    private ResourceDatabase _resourceDatabase;
    private Flag _flag;
    private bool _isBuildPriority = false;

    public event Action<Store, Unit> BuildingUnitArrived;
    public event Action<Flag> FlagReleased;

    public Vector3 BuildLocalPosition => _flag.transform.localPosition;

    public bool CanBuildNewStore => _unitCommander.UnitsCount >= _minUnitsCountForBuilding;
    public bool HaveFlag => _flag != null;

    private void Awake()
    {
        _unitCommander = GetComponent<StoreUnitCommander>();
    }

    private void Start()
    {
        _mark.gameObject.SetActive(false);
    }

    private void Update()
    {
        _scanner.Scan();
    }

    private void OnEnable()
    {
        _unitCommander.TaskCompleted += OnTaskCompleted;

        _counter.StoreAmountReached += OnStoreAmountReached;
        _counter.UnitAmountReached += OnUnitAmountReached;

        if (_resourceDatabase != null)
        {
            _scanner.ResourceFound += _resourceDatabase.AddResource;
            _resourceDatabase.ResourceAdded += SetNewTask;
        }
    }

    private void OnDisable()
    {
        _counter.StoreAmountReached -= OnStoreAmountReached;
        _counter.UnitAmountReached -= OnUnitAmountReached;
        _unitCommander.TaskCompleted -= OnTaskCompleted;
        _scanner.ResourceFound -= _resourceDatabase.AddResource;
        _resourceDatabase.ResourceAdded -= SetNewTask;
    }

    public void Initialize(int startUnitsCount, UnitSpawner unitSpawner, ResourceDatabase resourceDatabase, Button scanButton, Unit startUnit = null)
    {
        _scanner.Initialize(scanButton);
        _unitCommander.Initialize(unitSpawner, startUnitsCount, startUnit, _color);
        _resourceDatabase = resourceDatabase;

        _scanner.ResourceFound += _resourceDatabase.AddResource;
        _resourceDatabase.ResourceAdded += SetNewTask;
    }

    public void ActivateBuildingMark()
    {
        _mark.gameObject.SetActive(true);
    }

    public void SetFlag(Flag flag)
    {
        if (_flag != null)
        {
            FlagReleased?.Invoke(_flag);
            _flag = flag;
            _unitCommander.ChangeFlagPosition(_flag.transform.position);
        }
        else
        {
            _flag = flag;
            _counter.SetStoreCreatingPriority();
        }

        _flag.ChangeColor(_color);
    }

    public void BeforeChangeFlagPosition()
    {
        _unitCommander.PauseUnitBuilder();
    }

    public void ChangeColor(Color color)
    {
        _color = color;

        if (_mark.TryGetComponent(out ImageColorChanger markColorChanger))
            markColorChanger.ChangeColor(_color);

        if (_counter.TryGetComponent(out ImageColorChanger counterColorChanger))
            counterColorChanger.ChangeColor(_color);
    }

    private void OnStoreAmountReached()
    {
        _counter.SetUnitCreatingPriority();
        _counter.SpendStoreCost();
        _isBuildPriority = true;
        SetNewTask();
    }

    private void OnUnitAmountReached()
    {
        if (_unitCommander.TryCreateUnit())
            _counter.SpendUnitCost();
    }

    private void OnTaskCompleted(Resource resource, Unit unit)
    {
        if (resource == null)
        {
            OnUnitArrivedBuilding(unit);
        }
        else
        {
            _resourceDatabase.RemoveResource(resource);
            _counter.IncreaseResources();
        }

        SetNewTask();
    }

    private void OnUnitArrivedBuilding(Unit unit)
    {
        BuildingUnitArrived?.Invoke(this, unit);
        _unitCommander.RemoveUnit(unit);
        FlagReleased?.Invoke(_flag);
        _flag = null;
        _mark.gameObject.SetActive(false);
    }

    private void SetNewTask()
    {
        if (_unitCommander.HaveFreeUnits() == false)
            return;

        if (_isBuildPriority)
        {
            _isBuildPriority = false;
            _unitCommander.AddBuildTask(_flag.transform.position);
            return;
        }

        if (_resourceDatabase.TryGetResource(out Resource resource, this))
            _unitCommander.AddResourceTask(resource);
    }
}