using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserInput : MonoBehaviour
{
    public Vector3 MousePosition => Input.mousePosition;

    public event Action<Vector3> MouseClicked;

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
            MouseClicked?.Invoke(Input.mousePosition);
    }
}
