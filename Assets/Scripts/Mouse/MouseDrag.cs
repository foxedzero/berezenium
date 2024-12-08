using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private MouseEvent OnMouseBeginDrag;
    [SerializeField] private MouseEvent OnMouseDrag;
    [SerializeField] private MouseEvent OnMouseEndDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnMouseBeginDrag.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnMouseDrag.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnMouseEndDrag.Invoke(eventData);
    }
}
