using UnityEngine;

public class CanvasRotator : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
