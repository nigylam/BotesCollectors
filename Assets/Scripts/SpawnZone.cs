using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    private bool _isTaken = false;
    private Resource _resource;

    public bool IsTaken => _isTaken;

    private void OnEnable()
    {
        if (_resource != null)
            _resource.Took += Free;
    }

    private void OnDisable()
    {
        if (_resource != null)
            _resource.Took -= Free;
    }

    public void SetResource(Resource resource)
    {
        _resource = resource;
        _isTaken = true;

        _resource.Took += Free;
    }

    public void Free() 
    {
        _resource.Took -= Free;
        _isTaken = false; 
    }
}
