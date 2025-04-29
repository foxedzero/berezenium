using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CitySally;
using static UnityEngine.Rendering.DebugUI;

public class SallyPoint : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private GameObject Indicator;
    [SerializeField] private Vector2 Target;
    [SerializeField] private Image Model;
    [SerializeField] private CitySally.Sally Sally;
    private bool Move = false;

    private SallyPanel SallyPanel = null;

    public CitySally.Sally _Sally
    {
        get
        {
            return Sally;
        }
        set
        {
            if(Sally != null && Sally != value)
            {
                Sally = value;
                RectTransform.anchoredPosition = value._Position * 100f;
            }
            else
            {
                Target = value._Position * 100f;
                Move = true;
            }
        }
    }

    public void SetInfo(SallyPanel sallyPanel, CitySally.Sally sally)
    {
        SallyPanel = sallyPanel;
        RectTransform.anchoredPosition = sally._Position * 100f;
        sally.OnChanges += CheckSallyState;
        Sally = sally;
    }

    private void Update()
    {
        if (!Move)
        {
            return;
        }

        float distance = Vector2.Distance(Target, RectTransform.anchoredPosition);
        if(distance > 0)
        {
            RectTransform.anchoredPosition += (Target - RectTransform.anchoredPosition).normalized * Mathf.Max(10, distance) * Time.unscaledDeltaTime;

            if(Vector2.Distance(Target, RectTransform.anchoredPosition) > distance)
            {
                RectTransform.anchoredPosition = Target;
                Move = false;
            }
        }
    }

    public void CheckSallyState()
    {
        bool zero = true;
        foreach (Bear bear in Sally._Bears)
        {
            if (bear._Health > 1)
            {
                zero = false;
                break;
            }
        }
        if (zero)
        {
            Model.color = new Color(1, 0, 0);
            return;
        }

        if (Sally._AllowMove)
        {
            Model.color = new Color(0, 1, 0);
        }
        else
        {
            Model.color = new Color(1, 1, 0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SallyPanel.ClickSally(Sally))
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            SallyPanel.OpenSally(Sally);
        }
        else
        {
            string[] variants = new string[] { "Выбрать", (Sally._AllowMove ? "Остановиться" : "Продолжить путь") };
            int[] indexes = new int[] { 0, 1 };
            if (Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y)
            {
                variants = StaticTools.ExpandMassive(variants, new string[] { "Снарядить", "Расформировать" });
                indexes = StaticTools.ExpandMassive(indexes, new int[] { 2, 3 });
            }

            UserInteract.AskVariants($"{Sally._Name}", variants, indexes, Action);
        }
    }
    public void Action(int index)
    {
        switch (index)
        {
            case 0:
                SallyPanel.OpenSally(Sally);
                break;
            case 1:
                if (Sally._Bears.Length == 0)
                {
                    UserInteract.AskMessage("Нельзя продолжить путь", "Отряд пуст, чтобы начать вылазку назначьте медведей.");
                    return;
                }
                else if (Sally._Path == null || Sally._Path.Length == 0)
                {
                    UserInteract.AskMessage("Нельзя продолжить путь", "Местоположение отряда совпадает с точкой назначения.");
                    return;
                }

                int foodHour = Mathf.FloorToInt(25 * Sally._Honey / (Sally._Bears.Length * 3));
                int moveTime = Mathf.CeilToInt(Sally._Distance / Sally._TeamSpeed / CityTime._DaySection);

                if (foodHour < moveTime && !Sally._AllowMove)
                {
                    UserInteract.AskConfirm("Вы уверены ?", "Запасов провианта не хватит на проложенный маршрут.\nСоветуем достаточно снарядить отряд.\nВы хотите отправить отряд ?", AllowMove);
                }
                else
                {
                    AllowMove(true);
                }
                break;
            case 2:
                if (Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y)
                {
                    Sally.AutoSupply();
                }
                break;
            case 3:
                if (Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y)
                {
                    City._CitySally.RegisterSally(Sally, true);

                    foreach (Bear bear in Sally._Bears)
                    {
                        bear._Sally = null;
                    }

                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Sally._Name}", (int)(City._Time._WorldTime / 60), Sally._Honey, CityStorage.ResourceType.Honey));
                    City._Foodstream._StoredFood += Sally._Honey;
                    if (Sally._Snowrunner)
                    {
                        City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Sally._Name}", (int)(City._Time._WorldTime / 60), 1, CityStorage.ResourceType.Snowrunners));

                        City._Storage._Snowrunners++;
                    }
                }
                break;
        }
    }
    public void AllowMove(bool answer)
    {
        if (answer)
        {
            Sally._AllowMove = !Sally._AllowMove;
        }
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
