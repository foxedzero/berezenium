using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IlusionHolders : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform RectTransform;

    [SerializeField] private RectTransform HolderIndicator;
    [SerializeField] private float SizeY;
    [SerializeField] private int MaxIndex;
    private int Index = -1;

    public delegate void IntReturn(int value);
    private IntReturn OnPointed = null;
    private IntReturn OnClick = null;

    private PointerEventData PointerEventData = null;

    public int _MaxIndex
    {
        get
        {
            return MaxIndex;
        }
        set
        {
            MaxIndex = value;
        }
    }

    public void SetInfo(IntReturn onPoint, IntReturn onClick)
    {
        OnPointed = onPoint;
        OnClick = onClick;
    }

    private void Update()
    {
        if (PointerEventData != null)
        {
            Vector2 position = MouseToInsidePosition();
            int newIndex = Mathf.Min((int)(-position.y / SizeY), MaxIndex);

            if (newIndex != Index)
            {
                if (OnPointed != null)
                {
                    OnPointed.Invoke(newIndex);
                }
                Index = newIndex;

                HolderIndicator.anchoredPosition = new Vector2(0, -SizeY / 2f - Index * SizeY);
            }
        }
    }

    private Vector2 MouseToInsidePosition()
    {
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, PointerEventData.position, PointerEventData.pressEventCamera, out localMousePos);

        return localMousePos;
    }

    private void OnDisable()
    {
        PointerEventData = null;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(OnClick != null)
        {
            OnClick.Invoke(Index);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEventData = eventData;
        HolderIndicator.gameObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PointerEventData = null;
        HolderIndicator.gameObject.SetActive(false);
    }
}
