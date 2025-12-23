using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class ImageColorChanger : MonoBehaviour, IColorable
{
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void ChangeColor(Color color)
    {
        _image.color = color;
        _image.SetVerticesDirty();
        _image.SetMaterialDirty();
    }
}
