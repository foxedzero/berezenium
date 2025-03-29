using UnityEngine;

public class MainMenuBackground : MonoBehaviour
{
    [SerializeField] private RectTransform Canvas;
    [SerializeField] private MainMenuBackgroundElement[] Elements;
    [SerializeField] private Vector2 MousePosition;
    [SerializeField] private float MouseFollowSpeed;

    private void Update()
    {
        Vector2 mouseViewPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 mousePosition = new Vector2(Mathf.Clamp01(mouseViewPosition.x) * Canvas.sizeDelta.x, Mathf.Clamp01(mouseViewPosition.y) * Canvas.sizeDelta.y);

        float magnitude = (MousePosition - mousePosition).magnitude;
        MousePosition += (mousePosition - MousePosition).normalized * (Time.unscaledDeltaTime * MouseFollowSpeed * Mathf.Max(100 ,magnitude));
        if((mousePosition - MousePosition).magnitude > magnitude)
        {
            MousePosition = mousePosition;
        }


        foreach (MainMenuBackgroundElement element in Elements)
        {
            element.Move(MousePosition);
        }
    }
}