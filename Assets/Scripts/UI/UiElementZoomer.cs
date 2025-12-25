using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UiElementZoomer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 _baseScale;
    private Vector3 _basePosition;
    private float _zoomOffset = 0.2f;
    private float _zoomStrength = 2f;

    private void Awake()
    {
        _baseScale = transform.localScale;
        _basePosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ZoomIn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ZoomOut();
    }

    private void ZoomIn()
    {
        transform.localScale = _baseScale * _zoomStrength;
        transform.localPosition = new Vector3(_basePosition.x, _basePosition.y + _zoomOffset, _basePosition.z);
    }

    private void ZoomOut()
    {
        transform.localScale = _baseScale;
        transform.localPosition = _basePosition;
    }
}
