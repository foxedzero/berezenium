using UnityEngine;
using UnityEngine.EventSystems;

public class ColorHandler : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private RectTransform UserColorRect;
    [SerializeField] private UserColor UserColor;
    [SerializeField] private bool H;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 myPosition = RectTransform.anchoredPosition + UserColorRect.anchoredPosition - UserColorRect.sizeDelta / 2 - RectTransform.sizeDelta / 2;
       
        if (H)
        {
            UserColor.SetH(Mathf.Clamp01((eventData.position.x - myPosition.x) / RectTransform.sizeDelta.x));
        }
        else
        {
            UserColor.SetSV(Mathf.Clamp01((eventData.position.x - myPosition.x) / RectTransform.sizeDelta.x), Mathf.Clamp01((eventData.position.y - myPosition.y) / RectTransform.sizeDelta.y));
        }
    }
}
