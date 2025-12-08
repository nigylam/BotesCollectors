using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTraffic : MonoBehaviour
{
    private Queue<Unit> _baseQueue = new();
    private bool _canGo = true;
    private float _processDelay = 3f;

    private WaitForSeconds _processDelayWait;

    private void Awake()
    {
        _processDelayWait = new WaitForSeconds(_processDelay);
    }

    public void AskPass(Unit unit)
    {
        _baseQueue.Enqueue(unit);
        ProcessQueue();
    }

    public void AcceptPass()
    {
        _canGo = true;
        if (_baseQueue.Count > 0)
        {
            StartCoroutine(ProcessAfterWait());
        }
    }

    private void ProcessQueue()
    {
        if (_canGo == false || _baseQueue.TryDequeue(out Unit unit) == false)
            return;

        _canGo = false;
        GrantEnter(unit);
    }

    private void GrantEnter(Unit unit)
    {
        unit.GrantPass();
    }

    private IEnumerator ProcessAfterWait()
    {
        yield return _processDelayWait;

        ProcessQueue();
    }

}
