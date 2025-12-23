using UnityEngine;

[RequireComponent (typeof(UserInput))]
public class MouseFollower : MonoBehaviour
{
    private UserInput _userInput;

    private void Awake()
    {
        _userInput = GetComponent<UserInput>();
    }

    public bool TryGetHitPoint<T>(out Vector3 hitPoint) where T : Component
    {
        hitPoint = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(_userInput.MousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<T>(out _))
            {
                hitPoint = hit.point;
                return true;
            }
        }

        return false;
    }
}
