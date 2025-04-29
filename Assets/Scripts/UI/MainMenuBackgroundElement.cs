using UnityEngine;

public class MainMenuBackgroundElement : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private Vector2 Coefficient;
    [SerializeField] private Vector2 Offset;
    private Vector2 StartPosition;

    private void Start()
    {
        StartPosition = RectTransform.anchoredPosition;
    }

    public void Move(Vector2 mousePosition)
    {
        Vector2 direction = new Vector2((mousePosition.x - StartPosition.x) * Coefficient.x, (mousePosition.y - StartPosition.y) * Coefficient.y);

        RectTransform.anchoredPosition = direction + Offset;
    }
}