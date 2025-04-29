using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SallyMap : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SallyPanel SallyPanel;

    [SerializeField] private RectTransform RectTransform;
    private PointerEventData PointerEventData = null;

    public bool _MouseCaptured => PointerEventData != null;

    private void OnDisable()
    {
        PointerEventData = null;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SallyPanel.Click();
    }

    public Vector2 MouseToInsidePosition()
    {
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, PointerEventData.position, PointerEventData.pressEventCamera, out localMousePos);

        return localMousePos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEventData = eventData;
 //       HolderIndicator.gameObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PointerEventData = null;
    //    HolderIndicator.gameObject.SetActive(false);
    }
}
