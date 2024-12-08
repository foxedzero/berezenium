using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MouseEvent OnMouseEnter;
    [SerializeField] private MouseEvent OnMouseExit;
    private bool Entered = false;

    private void OnDisable()
    {
        if (Entered)
        {
            OnMouseExit.Invoke(null);
            Entered = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter.Invoke(eventData);
        Entered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit.Invoke(eventData);
        Entered = false;
    }
}
