using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private MouseEvent OnMouseDown;
    [SerializeField] private MouseEvent OnMouseUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnMouseDown.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnMouseUp.Invoke(eventData);
    }
}
