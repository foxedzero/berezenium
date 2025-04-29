using UnityEngine;
using static CitySally;
using UnityEngine.EventSystems;

public class SallyContentHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private GameObject Indicator;
    [SerializeField] private Tipper Tipper;
    private CitySally.TileContent TileContent;

    private SallyPanel SallyPanel = null;

    public void SetInfo(CitySally.TileContent content, SallyPanel sallyPanel)
    {
        RectTransform.anchoredPosition = new Vector2(content.Position.x * 100f + 50, content.Position.y * 100f + 50);

        TileContent = content;
        SallyPanel = sallyPanel;

        string info = $"Медведи:";
        foreach(Bear.Kasta kasta in TileContent.BearsKast)
        {
            info += $"\n{kasta}";
        }
        info += "\n";

        if (content.EnergyHoney > 0)
        {
            info += $"\nЭнергомёд: {content.EnergyHoney}";
        }
        if (content.Honey > 0)
        {
            info += $"\nМёд: {content.Honey}";
        }
        if (content.Wood > 0)
        {
            info += $"\nДревесина: {content.Wood}";
        }
        if (content.Metal > 0)
        {
            info += $"\nМеталл: {content.Metal}";
        }
        if (content.Berezenium > 0)
        {
            info += $"\nБерезениум: {content.Berezenium}";
        }
        if (content.Robots > 0)
        {
            info += $"\nРоботы: {content.Robots}";
        }

        Tipper._Info = info ;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SallyPanel.OpenSallyContent(TileContent);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Indicator.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Indicator.SetActive(false);
    }
}
