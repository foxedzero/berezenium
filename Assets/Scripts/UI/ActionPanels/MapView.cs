using UnityEngine;
using UnityEngine.UI;

public class MapView : ActionPanel
{
    [SerializeField] private Image Image;

    [SerializeField] private Slider ScaleSlider;
    [SerializeField] private RectTransform Map;
    [SerializeField] private float Sensitivity;
    private float Scale = 0.5f;

    public void SetInfo(Sprite sprite)
    {
        Image.sprite = sprite;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            ScaleSlider.value = Scale + Sensitivity;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            ScaleSlider.value = Scale - Sensitivity;
        }
    }

    public void SetScale(float value)
    {
        Scale = value;
        Map.localScale = Vector3.one * Mathf.Lerp(0.15f, 1.35f, value);
    }
}
