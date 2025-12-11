using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrafficZoneDetector : MonoBehaviour 
{
    public event Action<Unit> UnitEntered;
    public event Action<Unit> UnitExited;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Unit unit)) 
            UnitEntered?.Invoke(unit);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Unit unit))
            UnitExited?.Invoke(unit);
    }
}
