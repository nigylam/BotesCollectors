using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpawnZone : MonoBehaviour
{
    private bool _isTaken = false;

    public bool IsTaken => _isTaken;

    public void Take() { _isTaken = true; }

    public void Free() { _isTaken = false; }
}
