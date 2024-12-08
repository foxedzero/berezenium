using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class SimpleButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable] public class MyClickEvent : UnityEvent { }

    [FormerlySerializedAs("onClick")]
    [SerializeField]
    protected MyClickEvent m_OnClick = new MyClickEvent();

    [SerializeField] protected Graphic Graphic;
    [Space]
    [SerializeField] private Design.ColorType ActiveColor = Design.ColorType.Use;
    [SerializeField] private Design.ColorType DefaultColor = Design.ColorType.NoUse;

    private void OnDisable()
    {
        switch (DefaultColor)
        {
            case Design.ColorType.Main:
                Graphic.color = Design._MainColor;
                break;
            case Design.ColorType.Use:
                Graphic.color = Design._UseColor;
                break;
            case Design.ColorType.NoUse:
                Graphic.color = Design._NoUseColor;
                break;
            case Design.ColorType.Informational:
                Graphic.color = Design._InformationalColor;
                break;
        }
    }

    private void Awake()
    {
        if (Graphic == null)
        {
            Graphic = GetComponent<Graphic>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_OnClick.Invoke();

        SoundEffector.PlayUI(0);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (ActiveColor)
        {
            case Design.ColorType.Main:
                Graphic.color = Design._MainColor;
                break;
            case Design.ColorType.Use:
                Graphic.color = Design._UseColor;
                break;
            case Design.ColorType.NoUse:
                Graphic.color = Design._NoUseColor;
                break;
            case Design.ColorType.Informational:
                Graphic.color = Design._InformationalColor;
                break;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        switch (DefaultColor)
        {
            case Design.ColorType.Main:
                Graphic.color = Design._MainColor;
                break;
            case Design.ColorType.Use:
                Graphic.color = Design._UseColor;
                break;
            case Design.ColorType.NoUse:
                Graphic.color = Design._NoUseColor;
                break;
            case Design.ColorType.Informational:
                Graphic.color = Design._InformationalColor;
                break;
        }
    }
}
