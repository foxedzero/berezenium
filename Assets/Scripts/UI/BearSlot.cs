using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UserContent;

public class BearSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform RectTransform;

    [SerializeField] private Tipper Tipper;

    [SerializeField] private Image HeadIcon;
    [SerializeField] private Image FaceIcon;
    [SerializeField] private Image BrowsIcon;
    [SerializeField] private Bear Bear;
    [SerializeField] private int Index;

    public delegate void IntReturn(int index);
    private IntReturn Asker = null;

    public RectTransform _RectTransform => RectTransform;

    private void OnDisable()
    {
        Bear.OnSmthChange -= UpdateTip;
    }

    public void SetInfo(Bear bear, int index, IntReturn ask)
    {
        Bear = bear;
        Index = index;
        Asker = ask;

        BearLook bearLook = null;
        if (bear is SuperBear)
        {
            bearLook = City._BearObjectioner.GetBearIcon(bear as SuperBear);
        }
        else
        {
            bearLook = City._BearObjectioner.GetBearIcon(Bear._Face, Bear._Brows, Bear._BodyColor);
        }

        if (bearLook.Face == null)
        {
            HeadIcon.sprite = bearLook.Head;
            HeadIcon.color = new Color(1, 1, 1, 1);
            FaceIcon.color = new Color(0, 0, 0, 0);
            BrowsIcon.color = new Color(0, 0, 0, 0);
        }
        else
        {
            HeadIcon.sprite = bearLook.Head;
            HeadIcon.color = bearLook.SkinColor;
            FaceIcon.sprite = bearLook.Face;
            BrowsIcon.sprite = bearLook.Brows;
            FaceIcon.color = new Color(1, 1, 1, 1);
            BrowsIcon.color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateTip()
    {
        string info = $"{Bear._Name}";
        info += $"\n\nЗдоровье: {Bear._Health}/10";
        info += $"\nСтресс: {(int)Bear._Stress}%";
        info += $"\nРаботоспособность: {(int)(Bear._Work * 100)}%";
        info += $"\nУровень тепла: {Bear._HeatLevel}";
        info += $"\nУсталость: {(int)(Bear._Tired * 100)}%";
        info += $"\n\nСпециализация: {Bear._Kasta}";
        info += $"\nРабота: {(Bear._Facility != null ? Bear._Facility._ConstructInfo.Name : "отсутствует")}";
        info += $"\nЖилище: {(Bear._Home != null ? Bear._Home._ConstructInfo.Name : "отсутствует")}";
        info += $"\nРасписание: {Bear._Schedule}#";
        Tipper._Info = info ;
    }

    public void Click()
    {
        Asker?.Invoke(Index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Bear.OnSmthChange += UpdateTip;
        UpdateTip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Bear.OnSmthChange -= UpdateTip;
    }
}
