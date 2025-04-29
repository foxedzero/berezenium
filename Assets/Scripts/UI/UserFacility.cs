
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UserBear;

public class UserFacility : ChooseVariant, IPointerDownHandler, IDragHandler, IPointerClickHandler, ICancelable
{
    public enum Sorting { Index, Kasta, Type, Fullness }
    public enum DisplayInfo { Index = 1, Kasta = 2, Type = 4, Fullness = 8 }

    private Facility[] Facilities = new Facility[0];

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

    public delegate void FacilityReturn(Facility facility);
    private FacilityReturn ToReturn = null;

    private Vector2 StartPosition = Vector2.zero;
    private bool Initialized = false;


    protected override void OnDestroy()
    {
        base.OnDestroy();

        City._DataBase.OnBearChanges -= UpdateInfo;
    }

    public void SetInfo(string label, FacilityReturn toReturn, Sorting sorting = Sorting.Index, string sortInfo = "", int display = 0, int[] exlude = null)
    {
        if (City._DataBase._Facilities.Length == 0)
        {
            Debug.LogError($"нет зданий");
            UserInteract.AskMessage("Нет зданий!", "У вас не построено ни одно здание, возможно стоит начать.");
            Destroy(gameObject);
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
            case Sorting.Kasta:
                SortText.text = "По специальности";
                break;
            case Sorting.Type:
                SortText.text = "По типу";
                break;
            case Sorting.Fullness:
                SortText.text = "По заполненности";
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
        Outing = UserInteract.AskVariants("Сортировка", new string[] { "По номеру", "По специальности", "По типу", "По заполненности" }, new int[] { 0, 1, 2, 3 }, SetFacilitySorting).gameObject;
    }
    public void SetFacilitySorting(int index)
    {
        SortBy = (Sorting)index;
        switch (SortBy)
        {
            case Sorting.Index:
                SortText.text = "По номеру";
                break;
            case Sorting.Kasta:
                SortText.text = "По специальности";
                break;
            case Sorting.Type:
                SortText.text = "По типу";
                break;
            case Sorting.Fullness:
                SortText.text = "По заполненности";
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
                Outing = UserInteract.AskVariants("Номер", new string[] { "По возрастанию", "По убыванию" }, new int[] { 0, 1 }, SetUpping).gameObject;
                break;
            case Sorting.Kasta:
                Outing = UserInteract.AskVariants("Специальность", new string[] { "Неопределено", "Пасечник", "Конструктор", "Программист", "Биоинженер", "Первопроходец" }, new int[] { 0, 1, 2, 3, 4, 5 }, SetKasta).gameObject;
                break;
            case Sorting.Type:
                Outing = UserInteract.AskVariants("Тип", new string[] { "По возрастанию", "По убыванию" }, new int[] { 0, 1 }, SetType).gameObject;
                break;
            case Sorting.Fullness:
                Outing = UserInteract.AskVariants("Здоровье", new string[] { "По возрастанию", "По убыванию" }, new int[] { 0, 1 }, SetUpping).gameObject;
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
            case Sorting.Kasta:
                SortInfo = "Неопределено";
                break;
            case Sorting.Type:
                SortInfo = "Возрастание";
                break;
            case Sorting.Fullness:
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

    public void SetUpping(int index)
    {
        if (index == 0)
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
    public void SetType(int index)
    {
        SortInfo = $"{(Constructor.ConstructCategory)index}";
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
        Facility[] facilities = null;

        if (Exlude == null || Exlude.Length == 0 || !Exluding)
        {
            facilities = new Facility[City._DataBase._Facilities.Length];
            System.Array.Copy(City._DataBase._Facilities, facilities, facilities.Length);
        }
        else
        {
            facilities = new Facility[City._DataBase._Facilities.Length - Exlude.Length];
            int index = 0;
            for (int i = 0; i < City._DataBase._Facilities.Length; i++)
            {
                if (!StaticTools.Contains(Exlude, i))
                {
                    facilities[index] = City._DataBase._Facilities[i];
                    index++;
                }
            }
        }

        if (facilities.Length == 0)
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

        if (SortBy != Sorting.Index)
        {
            for (int i = 0; i < facilities.Length; i++)
            {
                int high = i;

                for (int ii = i + 1; ii < facilities.Length; ii++)
                {
                    bool breakingBad = false;

                    switch (SortBy)
                    {
                        case Sorting.Kasta:
                            switch (SortInfo)
                            {
                                case "Неопределено":
                                    if (facilities[ii]._RequiredKasta == Bear.Kasta.Неопределено)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Пасечник":
                                    if (facilities[ii]._RequiredKasta == Bear.Kasta.Пасечник)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Конструктор":
                                    if (facilities[ii]._RequiredKasta == Bear.Kasta.Конструктор)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Программист":
                                    if (facilities[ii]._RequiredKasta == Bear.Kasta.Программист)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Биоинженер":
                                    if (facilities[ii]._RequiredKasta == Bear.Kasta.Биоинженер)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Первопроходец":
                                    if (facilities[ii]._RequiredKasta == Bear.Kasta.Первопроходец)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                            }
                            if (facilities[ii]._RequiredKasta.GetHashCode() < facilities[high]._RequiredKasta.GetHashCode())
                            {
                                high = ii;
                            }
                            break;
                        case Sorting.Type:
                            switch (SortInfo)
                            {
                                case "Электроэнергия":
                                    if (facilities[ii]._ConstructInfo.Category == Constructor.ConstructCategory.Электроэнергия)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Жилище":
                                    if (facilities[ii]._ConstructInfo.Category == Constructor.ConstructCategory.Жилище)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Производство":
                                    if (facilities[ii]._ConstructInfo.Category == Constructor.ConstructCategory.Производство)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Добыча":
                                    if (facilities[ii]._ConstructInfo.Category == Constructor.ConstructCategory.Добыча)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Пища":
                                    if (facilities[ii]._ConstructInfo.Category == Constructor.ConstructCategory.Пища)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Медицина":
                                    if (facilities[ii]._ConstructInfo.Category == Constructor.ConstructCategory.Медицина)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                                case "Прочее":
                                    if (facilities[ii]._ConstructInfo.Category == Constructor.ConstructCategory.Прочее)
                                    {
                                        high = ii;
                                        breakingBad = true;
                                    }
                                    break;
                            }
                            if (facilities[ii]._ConstructInfo.Category.GetHashCode() < facilities[high]._ConstructInfo.Category.GetHashCode())
                            {
                                high = ii;
                            }
                            break;
                        case Sorting.Fullness:
                            if (SortInfo == "Возрастание")
                            {
                                if (facilities[ii]._AssignedBears.Length > facilities[high]._AssignedBears.Length)
                                {
                                    high = ii;
                                }
                            }
                            else
                            {
                                if (facilities[ii]._AssignedBears.Length < facilities[high]._AssignedBears.Length)
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

                Facility buffer = facilities[i];
                facilities[i] = facilities[high];
                facilities[high] = buffer;
            }
        }
        else
        {
            if (SortInfo != "Возрастание")
            {
                System.Array.Reverse(facilities);
            }
        }

        Facilities = facilities;

        string[] info = new string[facilities.Length];

        int display = Display | UserDisplay;
        switch (SortBy)
        {
            case Sorting.Index:
                display |= DisplayInfo.Index.GetHashCode();
                break;
            case Sorting.Kasta:
                display |= DisplayInfo.Kasta.GetHashCode();
                break;
            case Sorting.Type:
                display |= DisplayInfo.Type.GetHashCode();
                break;
            case Sorting.Fullness:
                display |= DisplayInfo.Fullness.GetHashCode();
                break;
        }
        bool[] displayInfo = StaticTools.FromByteBool(display);
        for (int i = 0; i < facilities.Length; i++)
        {
            info[i] = $"{facilities[i]._ConstructInfo.Name}" +
               $"{(displayInfo[0] ? $"  Id: {StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])} " : "")}" +
               $"{(displayInfo[1] ? $"  Спец: {facilities[i]._RequiredKasta} " : "")}" +
               $"{(displayInfo[2] ? $"  Тип: {facilities[i]._ConstructInfo.Category} " : "")}" +
               $"{(displayInfo[3] ? $"  Медв: {facilities[i]._Bears.Length}/{facilities[i]._AssignedBears.Length} " : "")}";
        }

        RectTransform content = Instantiate(ContentPrefab, List).GetComponent<RectTransform>();

        foreach (RectTransform content1 in Contents)
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
            Label.text = "Выбрать здание";
        }
        width = Mathf.Max(150, Label.preferredWidth + 40);

        Variants = new Variant[facilities.Length];
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

                if (i == 0)
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

            foreach (RectTransform rectTransform in Sizing)
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
            ToReturn.Invoke(Facilities[index]);
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
        Outing = UserInteract.AskVariants("", new string[] { $"{(OutClose ? "Не закрывать при аутклике" : "Закрывать при аутклике")}", $"{(Exluding ? "Не исключать здания" : "Исключать здания")}", "Сбросить отображения", "Отображать всё", $"{(displayInfo[0] ? "Не отображать" : "Отображать")} Номер", $"{(displayInfo[1] ? "Не отображать" : "Отображать")} Специализацию", $"{(displayInfo[2] ? "Не отображать" : "Отображать")} Тип", $"{(displayInfo[3] ? "Не отображать" : "Отображать")} Заполненность" },
            new int[] { -4, -3, -2, -1, 0, 1, 2, 3 }, ToggleDisplayInfo).gameObject;

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
