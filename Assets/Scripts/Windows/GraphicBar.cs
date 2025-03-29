using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphicBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private Image Image;
    [SerializeField] private Tipper Tipper;
    [SerializeField] private Text Hours;
        
    public RectTransform _RectTransform => RectTransform;
    public Tipper _Tipper => Tipper;
    public Text _Hours => Hours;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Image.color = new Color(0.6f, 0.6f, 0.6f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Image.color = new Color(0.9f, 0.9f, 0.9f);
    }
}
