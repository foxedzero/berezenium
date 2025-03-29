
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UserBear;

public class UserBear : ChooseVariant, IPointerDownHandler, IDragHandler, IPointerClickHandler, ICancelable
{
    public enum Sorting { Index, Name, Kasta, Work, Facility, Health, Stress, Tired, Schedule }
    public enum DisplayInfo {Index = 1, Kasta = 2, Work = 4, Facility = 8, Health = 16, Stress = 32, Tired = 64, Schedule = 128}

    private Bear[] Bears = new Bear[0];

    [SerializeField] private Text SortText;
    [SerializeField] private Text SortInfoText;

    [SerializeField] private RectTransform[] Sizing;

    private int[] Exlude = null;

    private RectTransform[] Contents = new RectTransform[0];

    private Sorting SortBy = Sorting.Index;
    private string SortInfo = "";
    private int Display = 0;
    static private int UserDisplay = 0;
    static private bool Exluding = true;
    static private bool OutClose = true;
     private GameObject Outing = null;

    public delegate void BearReturn(Bear bear);
    private BearReturn ToReturn = null;

    private Vector2 StartPosition = Vector2.zero;
    private bool Initialized = false;

    private bool NoWorkDoHome = false;
    public bool _NoWorkDoHome
    {
        get
        {
            return NoWorkDoHome;
        }
        set
        {
            NoWorkDoHome = value;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        City._DataBase.OnBearChanges -= UpdateInfo;
    }

    public void SetInfo(string label, BearReturn toReturn, Sorting sorting = Sorting.Index, string sortInfo = "", int display = 0, int[] exlude = null)
    {
        if(City._DataBase._Bears.Length == 0)
        {
            Debug.LogError($"нет медведей");
            Destroy(gameObject);
            UserInteract.AskMessage("Нет медведей!", "У вас нет ни единого медведя, стоит все же открыть капсулы и спасти их.");
            return;
        }

        Label.text = label;
        ToReturn = toReturn;
        Display = display;
        SortBy = sorting;
        Exlude = exlude;
        switch (SortBy)
        {
            case Sorting.Index:
                SortText.text = "По номеру";
                break;
            case Sorting.Name:
                SortText.text = "По имени";
                break;
            case Sorting.Kasta:
                SortText.text = "По специализации";
                break;
            case Sorting.Work:
                SortText.text = "По работоспособности";
                break;
            case Sorting.Facility:
                SortText.text = "По назначению в здание";
                break;
            case Sorting.Health:
                SortText.text = "По здоровью";
                break;
            case Sorting.Stress:
                SortText.text = "По стрессу";
                break;
            case Sorting.Tired:
                SortText.text = "По усталости";
                break;
            case Sorting.Schedule:
                SortText.text = "По расписанию";
                break;
        }

        if (sortInfo == "")
        {
            SetDefaultInfo();
        }
        else
        {
            SortInfo = sortInfo;
            SortInfoText.text = sortInfo;
        }

        City._DataBase.OnBearChanges += UpdateInfo;
        UpdateInfo();

        CancelQueue.Register(this, false);
    }

    public void EditSorting()
    {
        Outing = UserInteract.AskVariants("Сортировка", new string[] {"По номеру", "По имени" , "По специализации", "По работоспособности", "По назначению в здание", "По здоровью", "По стрессу", "По усталости", "По расписанию" }, new int[] {0, 1, 2, 3, 4, 5, 6,7, 8}, SetBearSorting).gameObject;
    }
    public void SetBearSorting(int index)
    {
        SortBy = (Sorting)index;
        switch (SortBy)
        {
            case Sorting.Index:
                SortText.text = "По номеру";
                break;
            case Sorting.Name:
                SortText.text = "По имени";
                break;
            case Sorting.Kasta:
                SortText.text = "По специализации";
                break;
            case Sorting.Work:
                SortText.text = "По работоспособности";
                break;
            case Sorting.Facility:
                SortText.text = "По назначению в здание";
                break;
            case Sorting.Health:
                SortText.text = "По здоровью";
                break;
            case Sorting.Stress:
                SortText.text = "По стрессу";
                break;
            case Sorting.Tired:
                SortText.text = "По усталости";
                break;
            case Sorting.Schedule:
                SortText.text = "По расписанию";
                break;
        }

        SetDefaultInfo();
        UpdateInfo();
    }

    public void EditSortInfo()
    {
        switch (SortBy)
        {
            case Sorting.Index:
                Outing = UserInteract.AskVariants("Номер", new string[] {"По возрастанию", "По убыванию"}, new int[] {0, 1}, SetUpping).gameObject;
                break;
            case Sorting.Name:
                Outing = UserInteract.AskInput("для алфавитной сортировки оставьте строку пустой", SetSortInfo).gameObject;
                break;
            case Sorting.Kasta:
                Outing = UserInteract.AskVariants("Специализация", new string[] { "Неопределено", "Пасечник", "Конструктор", "Программист", "Биоинженер", "Первопроходец" }, new int[] { 0, 1, 2, 3, 4, 5 }, SetKasta).gameObject;
                break;
            case Sorting.Work:
                Outing = UserInteract.AskVariants("Работоспособность", new string[] { "По возрастанию", "По убыванию" }, new int[] { 0, 1 }, SetUpping).gameObject;
                break;
            case Sorting.Facility:
                Outing = UserInteract.AskVariants("Здание", new string[] { "Работа", "Жилище" }, new int[] { 0, 1 }, SetFacility).gameObject;
                break;
            case Sorting.Health:
                Outing = UserInteract.AskVariants("Здоровье", new string[] { "По возрастанию", "По убыванию" }, new int[] { 0, 1 }, SetUpping).gameObject;
                break;
            case Sorting.Stress:
                Outing = UserInteract.AskVariants("Стресс", new string[] { "По возрастанию", "По убыванию" }, new int[] { 0, 1 }, SetUpping).gameObject;
                break;
            case Sorting.Tired:
                Outing = UserInteract.AskVariants("Усталость", new string[] { "По возрастанию", "По убыванию" }, new int[] { 0, 1 }, SetUpping).gameObject;
                break;
            case Sorting.Schedule:
                Outing = UserInteract.AskVariants("Расписание", new string[] { "По возрастанию", "По убыванию" }, new int[] { 0, 1 }, SetUpping).gameObject;
                break;
        }
    }
    public void SetDefaultInfo()
    {
        switch (SortBy)
        {
            case Sorting.Index:
                SortInfo = "Возрастание";
                break;
            case Sorting.Name:
                SortInfo = "";
                break;
            case Sorting.Kasta:
                SortInfo = "Неопределено";
                break;
            case Sorting.Work:
                SortInfo = "Возрастание";
                break;
            case Sorting.Facility:
                SortInfo = "Работа";
                break;
            case Sorting.Health:
                SortInfo = "Возрастание";
                break;
            case Sorting.Stress:
                SortInfo = "Возрастание";
                break;
            case Sorting.Tired:
                SortInfo = "Возрастание";
                break;
            case Sorting.Schedule:
                SortInfo = "Возрастание";
                break;
        }
        SortInfoText.text = SortInfo;
    }
    public void SetSortInfo(string info)
    {
        SortInfo = info;
        SortInfoText.text = SortInfo;

        UpdateInfo();
    }

    public void SetFacility(int index)
    {
        if (index == 0)
        {
            SortInfo = "Работа";
        }
        else
        {
            SortInfo = "Жилище";
        }
        SortInfoText.text = SortInfo;

        UpdateInfo();
    }
    public void SetUpping(int index)
    {
        if(index == 0)
        {
            SortInfo = "Возрастание";
        }
        else
        {
            SortInfo = "Убывание";
        }
        SortInfoText.text = SortInfo;

        UpdateInfo();
    }
    public void SetKasta(int index)
    {
        SortInfo = $"{(Bear.Kasta)index}";
        SortInfoText.text = SortInfo;
        UpdateInfo();
    }

    public void ToggleDisplayInfo(int display)
    {
        if (display == -4)
        {
            OutClose = !OutClose;
            UpdateInfo();
            return;
        }
        if (display == -3)
        {
            Exluding = !Exluding;
            UpdateInfo();
            return;
        }
        if (display == -2)
        {
            UserDisplay = 0;
            UpdateInfo();
            return;
        }
        else if (display == -1)
        {
            UserDisplay = 255;
            UpdateInfo();
            return;
        }

        bool[] bools = StaticTools.FromByteBool(UserDisplay);
        bools[display] = !bools[display];

        UserDisplay = StaticTools.ToByteBool(bools);

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        Bear[] bears = null;

        if(Exlude == null || Exlude.Length == 0 || !Exluding)
        {
            bears = new Bear[City._DataBase._Bears.Length];
            System.Array.Copy(City._DataBase._Bears, bears, bears.Length);
        }
        else
        {
            bears = new Bear[City._DataBase._Bears.Length - Exlude.Length];
            int index = 0;
            for(int i = 0; i < City._DataBase._Bears.Length; i++)
            {
                if(!StaticTools.Contains(Exlude, i))
                {
                    bears[index] = City._DataBase._Bears[i];
                    index++;
                }
            }
        }

        if(bears.Length == 0)
        {
            foreach (RectTransform content1 in Contents)
            {
                Destroy(content1.gameObject);
            }

            RectTransform.sizeDelta = new Vector2(200, 180);

            foreach (RectTransform rectTransform in Sizing)
            {
                rectTransform.sizeDelta = new Vector2(170, 40);
            }
            Contents = new RectTransform[0];

            Vector2 position1 = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);
            if (position1.x + RectTransform.sizeDelta.x / 2 > 1920)
            {
                if (position1.x - RectTransform.sizeDelta.x * 1.5f < 0)
                {
                    position1.x -= (position1.x + RectTransform.sizeDelta.x / 2) - 1920;
                }
                else
                {
                    position1.x -= RectTransform.sizeDelta.x;
                }
            }

            RectTransform.anchoredPosition = position1;

            Initialized = true;
            return;
        }

        int[] bearNamePriorities = new int[0];
        if (SortBy == Sorting.Name && SortInfo.Length > 0)
        {
            bearNamePriorities = new int[bears.Length];
            for (int i = 0; i < bears.Length; i++)
            {
                bearNamePriorities[i] = StaticTools.Match(SortInfo, bears[i]._Name);
            }
        }

        if (SortBy != Sorting.Index)
        {
            for (int i = 0; i < bears.Length; i++)
            {
                int high = i;

                bool breakingBad = false;

                switch (SortBy)
                {
                    case Sorting.Kasta:
                        switch (SortInfo)
                        {
                            case "Неопределено":
                                if (bears[high]._Kasta == Bear.Kasta.Неопределено)
                                {
                                    breakingBad = true;
                                }
                                break;
                            case "Пасечник":
                                if (bears[high]._Kasta == Bear.Kasta.Пасечник)
                                {
                                    breakingBad = true;
                                }
                                break;
                            case "Конструктор":
                                if (bears[high]._Kasta == Bear.Kasta.Конструктор)
                                {
                                    breakingBad = true;
                                }
                                break;
                            case "Программист":
                                if (bears[high]._Kasta == Bear.Kasta.Программист)
                                {
                                    breakingBad = true;
                                }
                                break;
                            case "Биоинженер":
                                if (bears[high]._Kasta == Bear.Kasta.Биоинженер)
                                {
                                    breakingBad = true;
                                }
                                break;
                            case "Первопроходец":
                                if (bears[high]._Kasta == Bear.Kasta.Первопроходец)
                                {
                                    breakingBad = true;
                                }
                                break;
                        }
                        break;
                }

                if (!breakingBad)
                {
                    for (int ii = i + 1; ii < bears.Length; ii++)
                    {
                        switch (SortBy)
                        {
                            case Sorting.Name:
                                if (SortInfo.Length <= 0)
                                {
                                    if (bears[ii]._Name.CompareTo(bears[high]._Name) < 0)
                                    {
                                        high = ii;
                                    }
                                }
                                else
                                {
                                    if (bearNamePriorities[ii] > bearNamePriorities[high])
                                    {
                                        high = ii;
                                    }
                                }
                                break;
                            case Sorting.Work:
                                if (SortInfo == "Возрастание")
                                {
                                    if (bears[ii]._Work > bears[high]._Work)
                                    {
                                        high = ii;
                                    }
                                }
                                else
                                {
                                    if (bears[ii]._Work < bears[high]._Work)
                                    {
                                        high = ii;
                                    }
                                }
                                break;
                            case Sorting.Facility:
                                if (SortInfo == "Работа")
                                {
                                    if (bears[ii]._Facility != null)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                }
                                else
                                {
                                    if (bears[ii]._Home != null)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                }
                                break;
                            case Sorting.Kasta:
                                switch (SortInfo)
                                {
                                    case "Неопределено":
                                        if (bears[ii]._Kasta == Bear.Kasta.Неопределено)
                                        {
                                            high = ii;
                                            breakingBad = true;
                                        }
                                        break;
                                    case "Пасечник":
                                        if (bears[ii]._Kasta == Bear.Kasta.Пасечник)
                                        {
                                            high = ii;
                                            breakingBad = true;
                                        }
                                        break;
                                    case "Конструктор":
                                        if (bears[ii]._Kasta == Bear.Kasta.Конструктор)
                                        {
                                            high = ii;
                                            breakingBad = true;
                                        }
                                        break;
                                    case "Программист":
                                        if (bears[ii]._Kasta == Bear.Kasta.Программист)
                                        {
                                            high = ii;
                                            breakingBad = true;
                                        }
                                        break;
                                    case "Биоинженер":
                                        if (bears[ii]._Kasta == Bear.Kasta.Биоинженер)
                                        {
                                            high = ii;
                                            breakingBad = true;
                                        }
                                        break;
                                    case "Первопроходец":
                                        if (bears[ii]._Kasta == Bear.Kasta.Первопроходец)
                                        {
                                            high = ii;
                                            breakingBad = true;
                                        }
                                        break;
                                }
                                if (breakingBad == false && bears[ii]._Kasta.GetHashCode() < bears[high]._Kasta.GetHashCode())
                                {
                                    high = ii;
                                }
                                break;
                            case Sorting.Health:
                                if (SortInfo == "Возрастание")
                                {
                                    if (bears[ii]._Health < bears[high]._Health)
                                    {
                                        high = ii;
                                    }
                                }
                                else
                                {
                                    if (bears[ii]._Health > bears[high]._Health)
                                    {
                                        high = ii;
                                    }
                                }
                                break;
                            case Sorting.Schedule:
                                if (SortInfo == "Возрастание")
                                {
                                    if (bears[ii]._Schedule < bears[high]._Schedule)
                                    {
                                        high = ii;
                                    }
                                }
                                else
                                {
                                    if (bears[ii]._Schedule > bears[high]._Schedule)
                                    {
                                        high = ii;
                                    }
                                }
                                break;
                            case Sorting.Stress:
                                if (SortInfo == "Возрастание")
                                {
                                    if (bears[ii]._Stress < bears[high]._Stress)
                                    {
                                        high = ii;
                                    }
                                }
                                else
                                {
                                    if (bears[ii]._Stress > bears[high]._Stress)
                                    {
                                        high = ii;
                                    }
                                }
                                break;
                            case Sorting.Tired:
                                if (SortInfo == "Возрастание")
                                {
                                    if (bears[ii]._Tired < bears[high]._Tired)
                                    {
                                        high = ii;
                                    }
                                }
                                else
                                {
                                    if (bears[ii]._Tired > bears[high]._Tired)
                                    {
                                        high = ii;
                                    }
                                }
                                break;
                        }

                        if (breakingBad)
                        {
                            break;
                        }
                    }
                }

                Bear buffer = bears[i];
                bears[i] = bears[high];
                bears[high] = buffer;

                if (SortBy == Sorting.Name && SortInfo.Length > 0)
                {
                    int sbuffer = bearNamePriorities[i];
                    bearNamePriorities[i] = bearNamePriorities[high];
                    bearNamePriorities[high] = sbuffer;
                }
            }
        }
        else
        {
            if(SortInfo != "Возрастание")
            {
                System.Array.Reverse(bears);
            }
        }

        Bears = bears;

        string[] info = new string[bears.Length];

        int display = Display | UserDisplay;
        switch (SortBy)
        {
            case Sorting.Index:
                display |= DisplayInfo.Index.GetHashCode();
                break;
            case Sorting.Work:
                display |= DisplayInfo.Work.GetHashCode();
                break;
            case Sorting.Facility:
                    display |= DisplayInfo.Facility.GetHashCode();
                break;
            case Sorting.Health:
                display |= DisplayInfo.Health.GetHashCode();
                break;
            case Sorting.Stress:
                display |= DisplayInfo.Stress.GetHashCode();
                break;
            case Sorting.Tired:
                display |= DisplayInfo.Tired.GetHashCode();
                break;
            case Sorting.Schedule:
                display |= DisplayInfo.Schedule.GetHashCode();
                break;
            case Sorting.Kasta:
                display |= DisplayInfo.Kasta.GetHashCode();
                break;
        }
        bool[] displayInfo = StaticTools.FromByteBool(display);
        for (int i = 0; i < bears.Length; i++)
        {
             info[i] = $"{bears[i]._Name}" +
                $"{(displayInfo[1] ? $"  {bears[i]._Kasta}" : "")}" +
                $"{(displayInfo[0] ? $"  Id: {StaticTools.IndexOf(City._DataBase._Bears, bears[i])} " : "")}" +
                $"{(displayInfo[2] ? $"  Р: {(int)(bears[i]._Work * 100f)}%" : "")}" +
                 $"{(displayInfo[3] ? (bears[i]._Sally == null ? (NoWorkDoHome == false ? $"  МР: {(bears[i]._Facility != null ? bears[i]._Facility._ConstructInfo.Name : "нет")}" : $"  Ж: {(bears[i]._Home != null ? bears[i]._Home._ConstructInfo.Name : "нет")}") : $"  Вылазка: {bears[i]._Sally._Name}") : "")}" +
                $"{(displayInfo[4] ? $"  ОЗ: {bears[i]._Health}" : "")}" +
                $"{(displayInfo[5] ? $"  Стресс: {bears[i]._Stress}%" : "")}" +
                $"{(displayInfo[6] ? $"  Уст: {Mathf.RoundToInt(bears[i]._Tired * 100)}%" : "")}" +
                $"{(displayInfo[7] ? $"  Расп: {bears[i]._Schedule}" : "")}";

        }

        RectTransform content = Instantiate(ContentPrefab, List).GetComponent<RectTransform>();

        foreach(RectTransform content1 in Contents)
        {
            Destroy(content1.gameObject);
        }

        Contents = new RectTransform[1] { content };
        float[] widthes = new float[0];

        bool sectored = false;
        float width = 150;
        float yPosition = 175;

        if (Label.text.Length == 0)
        {
            Label.text = "Выбрать медведя";
        }
        width = Mathf.Max(150, Label.preferredWidth + 40);

        Variants = new Variant[bears.Length];
        if (info.Length > 1)
        {
            for (int i = 0; i < info.Length; i++)
            {
                Variant newVariant = Instantiate(VariantPrefab, content).GetComponent<Variant>();

                Variants[i] = newVariant;

                yPosition += 20;

                newVariant.SetInfo(this, info[i], i, i, -yPosition);

                if (newVariant._Width > width)
                {
                    width = newVariant._Width;
                }

                yPosition += 20;

                if (yPosition >= StaticTools.ScreenHeight - 50)
                {
                    content.sizeDelta = new Vector2(width, StaticTools.ScreenHeight);

                    widthes = StaticTools.ExpandMassive(widthes, width);

                    width = 150;
                    yPosition = 0;

                    content = Instantiate(ContentPrefab, List).GetComponent<RectTransform>();

                    Contents = StaticTools.ExpandMassive(Contents, content);

                    sectored = true;
                }
            }
        }
        else
        {
            Variant newVariant = Instantiate(VariantPrefab, content).GetComponent<Variant>();

            Variants[0] = newVariant;

            yPosition += 20;

            newVariant.SetInfo(this, info[0], 0, 0, -yPosition);

            if (newVariant._Width > width)
            {
                width = newVariant._Width;
            }

            yPosition += 20;
        }

        widthes = StaticTools.ExpandMassive(widthes, width);

        content.sizeDelta = new Vector2(width, yPosition);

        Vector2 position = RectTransform.anchoredPosition;

        if (sectored)
        {
            RectTransform.sizeDelta = new Vector2(StaticTools.Summ(widthes), StaticTools.ScreenHeight);

            if (!Initialized)
            {
                position = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);
            }

            float xPosition = 0;
            for (int i = 0; i < Contents.Length - 1; i++)
            {
                xPosition += widthes[i] / 2;

                Contents[i].anchoredPosition = new Vector2(xPosition, -25);

                if(i == 0)
                {
                    foreach (RectTransform rectTransform in Sizing)
                    {
                        rectTransform.sizeDelta = new Vector2(widthes[i] - 30, 40);
                    }
                }

                xPosition += widthes[i] / 2;
            }

            content.anchoredPosition = new Vector2(xPosition + width / 2, (StaticTools.ScreenHeight - yPosition) / 2 - 25);

            position.y = StaticTools.ScreenHeight / 2;
        }
        else
        {
            RectTransform.sizeDelta = new Vector2(width, yPosition);
            
            foreach(RectTransform rectTransform in Sizing)
            {
                rectTransform.sizeDelta = new Vector2(width - 30, 40);
            }

            if (!Initialized)
            {
                position = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);
            }

            content.anchoredPosition = new Vector2(width / 2, 0);

            if (position.y - yPosition / 2 < 0)
            {
                if (position.y + RectTransform.sizeDelta.y * 1.5f > Screen.height)
                {
                    position.y -= position.y - RectTransform.sizeDelta.y / 2;
                }
                else
                {
                    position.y += RectTransform.sizeDelta.y;
                }
            }
        }

        if (position.x + RectTransform.sizeDelta.x / 2 > 1920)
        {
            if (position.x - RectTransform.sizeDelta.x * 1.5f < 0)
            {
                position.x -= (position.x + RectTransform.sizeDelta.x / 2) - 1920;
            }
            else
            {
                position.x -= RectTransform.sizeDelta.x;
            }
        }

        RectTransform.anchoredPosition = position;

        Initialized = true;
    }

    public void ChangeExlude(bool answer)
    {
        if (answer)
        {
            Exluding = !Exluding;
            UpdateInfo();
        }
    }

    public override void Select(int index)
    {
        if (ToReturn != null)
        {
            ToReturn.Invoke(Bears[index]);
        }

        Destroy(gameObject);
    }

    public void Close()
    {
        Destroy(gameObject);
    }

    protected override void Update()
    {
        if (OutClose && Outing == null)
        {
            base.Update();
        }
    }

    public void Settings()
    {
        bool[] displayInfo = StaticTools.FromByteBool(UserDisplay);
        Outing = UserInteract.AskVariants("", new string[] { $"{(OutClose ? "Не закрывать при аутклике" : "Закрывать при аутклике")}", $"{(Exluding ? "Не исключать медведей" : "Исключать медведей")}", "Сбросить отображения", "Отображать всё", $"{(displayInfo[0] ? "Не отображать" : "Отображать")} Номер", $"{(displayInfo[1] ? "Не отображать" : "Отображать")} Специализацию", $"{(displayInfo[2] ? "Не отображать" : "Отображать")} Работоспособность", $"{(displayInfo[3] ? "Не отображать" : "Отображать")} Назначения", $"{(displayInfo[4] ? "Не отображать" : "Отображать")} Здоровье", $"{(displayInfo[5] ? "Не отображать" : "Отображать")} Стресс", $"{(displayInfo[6] ? "Не отображать" : "Отображать")} Усталость", $"{(displayInfo[0] ? "Не отображать" : "Отображать")} Расписание" },
            new int[] { -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7 }, ToggleDisplayInfo).gameObject;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Settings();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartPosition = RectTransform.anchoredPosition;
        OnDrag(eventData);

        MouseCaptured = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform.anchoredPosition = StartPosition + eventData.position - eventData.pressPosition;
    }
}
