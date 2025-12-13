using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTraffic : MonoBehaviour
{
    [SerializeField] private TrafficZoneDetector _waitingZone;
    [SerializeField] private TrafficZoneDetector _trafficZone;

    private Queue<Unit> _queue = new();
    private Unit _currentUnit;

    private void OnEnable()
    {
        _waitingZone.UnitEntered += OnUnitEnteredWaitingZone;
        _waitingZone.UnitExited += OnUnitExitedWaitingZone;
        _trafficZone.UnitExited += OnUnitExitedTrafficZone;
    }

    private void OnDisable()
    {
        _waitingZone.UnitEntered -= OnUnitEnteredWaitingZone;
        _waitingZone.UnitExited -= OnUnitExitedWaitingZone;
        _trafficZone.UnitExited -= OnUnitExitedTrafficZone;
    }

    private void OnUnitEnteredWaitingZone(Unit unit)
    {
        if (_queue.Contains(unit))
            return;

        StopUnit(unit);
        ProcessQueue();
    }

    private void OnUnitExitedWaitingZone(Unit unit)
    {
        if (_currentUnit == unit)
            _currentUnit = null;
    }

    private void StopUnit(Unit unit)
    {
        _queue.Enqueue(unit);
        unit.PauseMoving();
    }

    private void OnUnitExitedTrafficZone(Unit unit)
    {
        _currentUnit = null;
        ProcessQueue();
    }

    private void ProcessQueue()
    {
        if (_queue.Count == 0)
            return;

        if (_currentUnit == null)
        {
            _currentUnit = _queue.Dequeue();
            _currentUnit.ContinueMoving();
        }
    }
}