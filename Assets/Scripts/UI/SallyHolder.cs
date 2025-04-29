
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Constructor;

public class SallyHolder : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private Text Text;
    [SerializeField] private Tipper Tipper;
    private SallyPanel SallyPanel = null;
    [SerializeField] private CitySally.Sally Sally = null;

    public CitySally.Sally _Sally
    {
        get
        {
            return Sally;
        }
        set
        {
            if(Sally == value)
            {
                UpdateInfo();
                return;
            }

            if(Sally != null)
            {
                Sally.OnChanges -= UpdateInfo;
            }

            Sally = value;

            Sally.OnChanges += UpdateInfo;

            UpdateInfo();
        }
    }

    private void OnDestroy()
    {
        if (Sally != null)
        {
            Sally.OnChanges -= UpdateInfo;
        }
    }

    public void SetInfo(SallyPanel panel, float position)
    {
        SallyPanel = panel;
        RectTransform.anchoredPosition = new Vector2(0, position);
    }

    public void UpdateInfo()
    {
        int hours = Mathf.CeilToInt(Sally._Distance / Sally._TeamSpeed / CityTime._DaySection);
        if(hours < 0)
        {
            hours = 0;
        }

        if (!Sally._AllowMove)
        {
            Text.text = $"{Sally._Name}  <color=red>({hours} ч)</color>";
        }
        else
        {
            Text.text = $"{Sally._Name}  ({hours} ч)";
        }

        float hp = 0;
        float stress = 0;
        if(Sally._Bears.Length > 0)
        {
            foreach (Bear bear in Sally._Bears)
            {
                stress += bear._Stress;
                hp += bear._Health;
            }
            stress /= Sally._Bears.Length;
            hp /= Sally._Bears.Length;
        }

        string info = $"Координаты: x{Mathf.Round(Sally._Position.x * 10) / 10f} y{Mathf.Round(Sally._Position.y * 10) / 10f}\n" +
            $"Назначение: x{Mathf.FloorToInt(Sally._Position.x)} y{Mathf.FloorToInt(Sally._Position.y)}\n" +
            $"Расчётное время: {(Sally._TeamSpeed == 0 ? "∞" : hours)} ч\n" +
            $"В составе: {Sally._Bears.Length} медведей\n" +
            $"Провиант: {Sally._Honey}  ({(Sally._Bears.Length > 0 ? Mathf.FloorToInt(25 * Sally._Honey / (Sally._Bears.Length * 3)) : "∞")} ч)\n" +
            $"Здоровье: {hp} ед.\n" +
            $"Стресс: {  Mathf.RoundToInt(stress)}%\n" +
            $"Состояние: {(Sally._AllowMove ? "В пути" : "<color=red>Ожидание</color>")}";

        Tipper._Info = info;
    }

    public void Click()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            SallyPanel.OpenSally(Sally);
        }
        else
        {
            string[] variants = new string[] { "Выбрать", (Sally._AllowMove ? "Остановиться" : "Продолжить путь") };
            int[] indexes = new int[] { 0, 1 };
            Vector2Int position = new Vector2Int(Mathf.FloorToInt(Sally._Position.x), Mathf.FloorToInt(Sally._Position.y));

            if(position.x == CitySally.TownPoint.x && position.y == CitySally.TownPoint.y)
            {
                variants = StaticTools.ExpandMassive(variants, new string[] { "Снарядить", "Расформировать" });
                indexes = StaticTools.ExpandMassive(indexes, new int[] { 2, 3 });
            }
            if(SallyPanel._Current != null && SallyPanel._Current != Sally)
            {
                if(position.x == Mathf.FloorToInt(SallyPanel._Current._Position.x) && position.y == Mathf.FloorToInt(SallyPanel._Current._Position.y))
                {
                    variants = StaticTools.ExpandMassive(variants,  "Объединить отряды" );
                    indexes = StaticTools.ExpandMassive(indexes, 4);
                }
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
            case 4:
                if (SallyPanel._Current != null && SallyPanel._Current != Sally)
                {
                    Vector2Int position = new Vector2Int(Mathf.FloorToInt(Sally._Position.x), Mathf.FloorToInt(Sally._Position.y));
                    if (position.x == Mathf.FloorToInt(SallyPanel._Current._Position.x) && position.y == Mathf.FloorToInt(SallyPanel._Current._Position.y))
                    {
                        SallyPanel._Current.Combine(Sally);
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
}
