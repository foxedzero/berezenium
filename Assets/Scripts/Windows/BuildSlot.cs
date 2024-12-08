
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Constructor;

public class BuildSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image Backgorund;
    [SerializeField] private Image Icon;
    [SerializeField] private Tipper Tipper;
    private ConstractionWindow Window = null;
    private ConstructInfo Info = null;

    public void SetInfo(ConstractionWindow window, ConstructInfo info)
    {
        Window = window;
        Info = info;

        Icon.sprite = Info.Icon;
        Tipper._Info =  $"<size=26>{info.Name}</size>\n\nГабариты:    длина {info.Sizes.x}    ширина {info.Sizes.y}\nТребуется древесины: {info.WoodCost}\nТребуется металла: {info.MetalCost}\nТребуется березениума: {info.BerezenuimCost}\nНавык строительства: {info.BuildSkill}\nВремя строительства: {info.BuildWork} день\n\n{info.Description}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundEffector.PlayUI(0);
        Window.Construct(Info);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Backgorund.color = Design._UseColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Backgorund.color = Design._NoUseColor;
    }
}
