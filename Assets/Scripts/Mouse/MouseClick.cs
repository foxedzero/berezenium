using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MouseEvent OnMouseClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnMouseClick.Invoke(eventData);
    }
}
