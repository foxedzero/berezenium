using UnityEngine;
using UnityEngine.EventSystems;
using WindowInterfaces;

public class WindowDrag : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IDragHandler
{
    [SerializeReference] private Window Window;
    private Vector2 OffsetPosition = Vector2.zero;

    public virtual void MouseDown(PointerEventData eventData)
    {
        Window.Select();

        OffsetPosition = Window._Rect.anchoredPosition;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Window.Select();

            OffsetPosition = Window._Rect.anchoredPosition;
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Window.RightMouse();
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            (Window as IMoveable).Move(OffsetPosition + eventData.position - eventData.pressPosition);
        }
    }
}
