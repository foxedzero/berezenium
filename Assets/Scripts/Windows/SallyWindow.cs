
using UnityEngine;
using UnityEngine.UI;
using WindowInterfaces;

public class SallyWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private GameObject BearSlotPrefab;
    [SerializeField] private RectTransform AddBearButton;
    [SerializeField] private RectTransform Content;
    private BearSlot[] Slots = new BearSlot[0];

    [SerializeField] private Text Name;
    [SerializeField] private Text Path;
    [SerializeField] private Text Food;
    [SerializeField] private Text Snowrunner;
    [SerializeField] private Text AllowMoving;

    [SerializeField] private Tipper NameTip;
    [SerializeField] private Tipper PathTip;
    [SerializeField] private Tipper FoodTip;
    [SerializeField] private Tipper SnowrunnerTip;

    private SallyPanel Panel = null;

    [SerializeField] private GameObject[] TownActions;

    private Bear Selected = null;

    [SerializeField] private CitySally.Sally Sally;

    public CitySally.Sally _Sally => Sally;

    public override string _Label => $"Окно отряда"; 

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Panel.ShowWay(null);

        City._Foodstream.OnChanges -= UpdateFoodStorage;

        if (Sally != null)
        {
            Sally.OnChanges -= UpdateInfo;
        }
    }

    public void SetInfo(SallyPanel sallyPanel, CitySally.Sally sally)
    {
        if(Panel == null)
        {
            Panel = sallyPanel;
            City._Foodstream.OnChanges += UpdateFoodStorage;
        }

        if (Sally != null)
        {
            Sally.OnChanges -= UpdateInfo;
        }

        Sally = sally;

        if (Sally != null)
        {
            Sally.OnChanges += UpdateInfo;
            UpdateInfo();
        }
    }

    public void UpdateFoodStorage()
    {
        Food.text = $"Провиант: {Sally._Honey}  ({(Sally._Bears.Length > 0 ? Mathf.FloorToInt(25 * Sally._Honey / (Sally._Bears.Length * 3)) : "∞")} ч)  На складах: {City._Foodstream._StoredFood}";
    }

    public void UpdateInfo()
    {
        if(!StaticTools.Contains(City._CitySally._Sallies, Sally))
        {
            Destroy(gameObject);
            return;
        }

        Name.text = $"Название отряда: {Sally._Name}";
        Path.text = $"Маршрут: x{(int)(Sally._Position.x)} y{(int)(Sally._Position.y)} ---> x{(int)(Sally._Destination.x)} y{(int)(Sally._Destination.y)}  ({(Sally._TeamSpeed == 0 ? "∞" : Mathf.CeilToInt(Sally._Distance / Sally._TeamSpeed / CityTime._DaySection))} ч)";
        Food.text = $"Провиант: {Sally._Honey}  ({(Sally._Bears.Length > 0 ? Mathf.FloorToInt(25 * Sally._Honey/(Sally._Bears.Length * 3)) : "∞")} ч)  На складах: {City._Foodstream._StoredFood}";
        Snowrunner.text = $"Комплект снегоходов: {(Sally._Snowrunner ? "есть" : "нет")}";

        if (Sally._AllowMove)
        {
            AllowMoving.text = $"СТОП";
        }
        else
        {
            AllowMoving.text = $"В ПУТЬ !";
        }

        int count = Sally._Bears.Length;
        if (Slots.Length < count)
        {
            BearSlot[] newSlots = new BearSlot[count - Slots.Length];
            for (int i = 0; i < count - Slots.Length; i++)
            {
                BearSlot slot = Instantiate(BearSlotPrefab, Content).GetComponent<BearSlot>();
                newSlots[i] = slot;
            }

            Slots = StaticTools.ExpandMassive(Slots, newSlots);
        }
        else if (Slots.Length > count)
        {
            while (Slots.Length > count)
            {
                Destroy(Slots[Slots.Length - 1].gameObject);
                Slots = StaticTools.ReduceMassive(Slots, Slots.Length - 1);
            }
        }

        float x = 64;
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].SetInfo(Sally._Bears[i], i, SelectBear);
            Slots[i]._RectTransform.anchoredPosition = new Vector2(x, 0);
            x += 153;
        }

        bool inTown = Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y;
        foreach (GameObject townact in TownActions)
        {
            townact.SetActive(inTown);
        }

        if (inTown)
        {
            AddBearButton.anchoredPosition = new Vector2(x, 0);
            Content.sizeDelta = new Vector2(x + 64, 0);
        }
        else
        {
            AddBearButton.anchoredPosition = new Vector2(x, 0);
            Content.sizeDelta = new Vector2(x - 64, 0);
        }

        Panel.ShowWay(new SquareAStar(City._CitySally._Map, new Vector2Int(Mathf.FloorToInt(Sally._Position.x), Mathf.FloorToInt(Sally._Position.y)), new Vector2Int(Mathf.FloorToInt(Sally._Destination.x), Mathf.FloorToInt(Sally._Destination.y))));
    }

    public void SelectBear(int index)
    {
        Selected = Sally._Bears[index];

        if (Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y)
        {
            UserInteract.AskVariants("", new string[] { "Информация", "Исключить из отряда" }, new int[] { 0, 1 }, ActBear);
        }
        else
        {
            ActBear(0);
        }
    }
    public void ActBear(int index)
    {
        switch (index)
        {
            case 0:
                WindowCreator.CreateWindow<BearWindow>().SetInfo(Selected);
                break;
            case 1:
                if(Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y)
                {
                    Sally.RegisterBear(Selected, true);
                }
                else
                {
                    UserInteract.AskMessage("Нельзя исключить", "Медведи не бросят своего товарища одного в этой пустоши. \nЧтобы исключить медведя из группы, дойдите до поселения.");
                }
                break;
        }

        Selected = null;
    }

    public void DeleteSally()
    {
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

            NewTutorialSystem.Instance.SallyDeleted();

            Destroy(gameObject);
        }
        else
        {
            UserInteract.AskMessage("Расформировка невозможна", "Нельзя расформировать отряд вне поселения.");
        }
    }

    public void AllowMove()
    {
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
    }
    public void AllowMove(bool answer)
    {
        if (answer)
        {
            NewTutorialSystem.Instance.SendSally();

            Sally._AllowMove = !Sally._AllowMove;

            if (Sally._AllowMove)
            {
                AllowMoving.text = $"СТОП";
            }
            else
            {
                AllowMoving.text = $"В ПУТЬ !";
            }
        }
    }

    public void AutoSupply()
    {
        if (Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y)
        {
            Sally.AutoSupply();
        }
    }

    public void AddBear()
    {
        int[] exlude = new int[Sally._Bears.Length];
        for(int i = 0; i < exlude.Length; i++)
        {
            exlude[i] = StaticTools.IndexOf(City._DataBase._Bears, Sally._Bears[i]);
        }

        UserInteract.AskBear("Назачить медведя в отряд", AddBear, UserBear.Sorting.Kasta, Bear.Kasta.Первопроходец.ToString(), UserBear.DisplayInfo.Facility.GetHashCode(), exlude);
    }
    public void AddBear(Bear bear)
    {
        if (!(Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y))
        {
            return;
        }

        Sally.RegisterBear(bear, false);
    }

    public void SetName()
    {
        UserInteract.AskInput("Переименовать отряд", SetName);
    }
    public void SetName(string value)
    {
        Sally._Name = value;
    }

    public void SetHoney(bool remove)
    {
        if (!(Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y))
        {
            return;
        }

        if (remove)
        {
            int delta = Mathf.Min(Sally._Honey, 2);
            City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Sally._Name}", (int)(City._Time._WorldTime / 60), -delta, CityStorage.ResourceType.Honey));
            Sally._Honey -= delta;
            City._Foodstream._StoredFood += delta;
        }
        else
        {
            int delta = Mathf.Min((int)City._Foodstream._StoredFood, 2);
            City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Sally._Name}", (int)(City._Time._WorldTime / 60), delta, CityStorage.ResourceType.Honey));
            Sally._Honey += delta;
            City._Foodstream._StoredFood -= delta;
        }
    }

    public void SetSnowrunner()
    {
        if (!(Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y))
        {
            return;
        }

        if (Sally._Snowrunner)
        {
            City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Sally._Name}", (int)(City._Time._WorldTime / 60), 1, CityStorage.ResourceType.Snowrunners));

            Sally._Snowrunner = false;
            City._Storage._Snowrunners++;
        }
        else if(City._Storage._Snowrunners > 0)
        {
            City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Sally._Name}", (int)(City._Time._WorldTime / 60), -1, CityStorage.ResourceType.Snowrunners));

            Sally._Snowrunner = true;
            City._Storage._Snowrunners--;
        }
    }

    public void SetDestination(Vector2 destination)
    {
        Sally._Destination = destination;

        if (Mathf.FloorToInt(destination.x) == CitySally.TownPoint.x && Mathf.FloorToInt(destination.y) == CitySally.TownPoint.y)
        {
            NewTutorialSystem.Instance.SallyToBase();
        }
    }
}
