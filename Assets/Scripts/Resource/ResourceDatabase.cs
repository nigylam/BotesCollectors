using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDatabase : MonoBehaviour
{
    private Dictionary<Resource, Store> _foundResources = new();
    private List<Resource> _selectedResources = new();

    private List<Store> _stores = new();

    public event Action ResourceAdded;

    public void AddStore(Store store)
    {
        _stores.Add(store);

        if (_foundResources.Count > 0)
            RedistributeResources();
    }

    public void AddResource(Resource resource)
    {
        if (_foundResources.ContainsKey(resource) || _selectedResources.Contains(resource))
            return;

        _foundResources.Add(resource, GetNearestStore(resource));
        resource.Outline();

        ResourceAdded?.Invoke();
    }

    public bool TryGetResource(out Resource resource, Store requestingStore)
    {
        resource = null;

        if (_foundResources.Count == 0)
            return false;

        foreach (Resource resourceKey in _foundResources.Keys)
        {
            if (_foundResources[resourceKey] == requestingStore)
            {
                resource = resourceKey;
                _foundResources.Remove(resource);
                _selectedResources.Add(resource);
                return true;
            }
        }

        return false;
    }

    public void RemoveResource(Resource resource)
    {
        if (_selectedResources.Contains(resource))
            _selectedResources.Remove(resource);
    }

    private void RedistributeResources()
    {
        List<Resource> resources = new();
        resources.AddRange(_foundResources.Keys);
        _foundResources.Clear();

        foreach (var resource in resources)
            _foundResources.Add(resource, GetNearestStore(resource));
    }

    private Store GetNearestStore(Resource resource)
    {
        float minDistance = float.MaxValue;
        var nearestStore = _stores[0];

        foreach (var store in _stores)
        {
            float sqrDistance = Vector3.SqrMagnitude(resource.transform.position - store.transform.position);

            if (sqrDistance < minDistance)
            {
                minDistance = sqrDistance;
                nearestStore = store;
            }
        }

        return nearestStore;
    }
}
