using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDatabase : MonoBehaviour
{
    private List<Resource> _foundResources = new();
    private List<Resource> _selectedResources = new();

    public event Action ResourceAdded;

    public void AddResource(Resource resource)
    {
        if (_foundResources.Contains(resource) || _selectedResources.Contains(resource))
            return;

        _foundResources.Add(resource);
        resource.Outline();

        ResourceAdded?.Invoke();
    }

    public bool TryGetResource(out Resource resource, Base requestingBase)
    {
        if(_foundResources.Count > 0)
        {
            resource = GetNearestResource(requestingBase.transform);

            if (resource != null)
                return true;

            return false;
        }

        resource = null;
        return false;
    }

    public Resource GetNearestResource(Transform point)
    {
        if (_foundResources.Count == 0)
            return null;

        Resource nearestResource = _foundResources[0];

        foreach (Resource resource in _foundResources)
        {
            if (nearestResource == resource) 
                continue;

            if (Vector3.SqrMagnitude(resource.transform.position - point.position) < Vector3.SqrMagnitude(nearestResource.transform.position - point.position))
                nearestResource = resource;
        }

        _foundResources.Remove(nearestResource);
        _selectedResources.Add(nearestResource);

        return nearestResource;
    }

    public void RemoveResource(Resource resource)
    {
        if(_selectedResources.Contains(resource))
            _selectedResources.Remove(resource);
    }
}
