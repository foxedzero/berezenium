
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Constructor;

public class BuildSlot : MonoBehaviour
{
    [SerializeField] private Image Icon;
    [SerializeField] private Tipper Tipper;
    private ConstractionWindow Window = null;
    private ConstructInfo Info = null;

    public void SetInfo(ConstractionWindow window, ConstructInfo info)
    {
        Window = window;
        Info = info;

        Icon.sprite = Info.Icon;

        string tip = $"<size=26>{info.Name}</size>\n\nГабариты:    длина {info.Sizes.x}    ширина {info.Sizes.y}";
        if(info.WoodCost > 0)
        {
            tip += $"\nТребуется древесины: {info.WoodCost}";
        }
        if (info.MetalCost > 0)
        {
            tip += $"\nТребуется металла: {info.MetalCost}";
        }
        if (info.BerezenuimCost > 0)
        {
            tip += $"\nТребуется березениума: {info.BerezenuimCost}";
        }
        tip += $"\nРаботы для постройки: {info.BuildWork * 100}%\n\n{info.Description}";
        Tipper._Info = tip;
    }

    public void Click()
    {
        Window.Construct(Info);
    }
}
