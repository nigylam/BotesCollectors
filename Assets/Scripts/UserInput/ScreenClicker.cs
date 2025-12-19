using System;
using UnityEngine;

[RequireComponent(typeof(UserInput))]
public class ScreenClicker : MonoBehaviour
{
    private UserInput _userInput;

    public event Action<Store> StoreClicked;
    public event Action<Vector3> PlaneClicked;

    private void Awake()
    {
        _userInput = GetComponent<UserInput>();
    }

    private void OnEnable()
    {
        _userInput.MouseClicked += ClickObject;
    }

    private void OnDisable()
    {
        _userInput.MouseClicked -= ClickObject;
    }

    private void ClickObject(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out Store store))
            {
                StoreClicked?.Invoke(store);
            }
            else if (hit.collider.TryGetComponent(out Plane plane))
            {
                PlaneClicked?.Invoke(hit.point);
            }
        }
    }
}
