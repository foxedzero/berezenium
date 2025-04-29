using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MyButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable] public class MyClickEvent : UnityEvent { }

    [FormerlySerializedAs("onClick")]
    [SerializeField]
    protected MyClickEvent m_OnClick = new MyClickEvent();

    [SerializeField] protected Graphic Graphic;
    [Space]
    [SerializeField] private Color ActiveColor;
    [SerializeField] private Color DefaultColor;
    private bool Captured = false;

    public Color _DefaultColor
    {
        get
        {
            return DefaultColor;
        }
        set
        {
            DefaultColor = value;

            if (!Captured)
            {
                Graphic.color = DefaultColor;
            }
        }
    }
    public Color _ActiveColor
    {
        get
        {
            return ActiveColor;
        }
        set
        {
            ActiveColor = value;

            if (Captured)
            {
                Graphic.color = ActiveColor;
            }
        }
    }

    private void OnDisable()
    {
        Graphic.color = DefaultColor;
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
        Graphic.color = ActiveColor;
        Captured = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Graphic.color = DefaultColor;
        Captured = false;
    }
}
