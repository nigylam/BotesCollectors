using UnityEngine;

public class CanvasRotator : MonoBehaviour
{
    private void Start()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
