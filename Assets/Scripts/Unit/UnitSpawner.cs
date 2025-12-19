using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;

    public Unit Spawn(Transform homePoint, Transform storePoint)
    {
        Unit unit = Instantiate(_unitPrefab, homePoint);
        unit.Initialize(storePoint, homePoint.position);
        return unit;
    }
}
