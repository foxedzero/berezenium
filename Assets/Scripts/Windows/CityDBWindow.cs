using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using WindowInterfaces;

public class CityDBWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private RectTransform Content;

    [SerializeField] private IlusionHolders IlusionHolders;
    [SerializeField] private Text List;
    private Bear[] Bears = new Bear[0];
    private Facility[] Facilities = new Facility[0];

    [SerializeField] private Text FindObject;

    [SerializeField] private Text[] BearFilters;
    [SerializeField] private GameObject BearNameButton;
    [SerializeField] private GameObject BearIndexButton;
    [SerializeField] private GameObject BearFilterPage;

    [SerializeField] private Text[] FacilityFilters;
    [SerializeField] private GameObject FacilityNameButton;
    [SerializeField] private GameObject FacilityIndexButton;
    [SerializeField] private GameObject FacilityFilterPage;

    private bool FindBear = true;

    private BearWindow BearWindow = null;
    private int[] BearAge = new int[2] { 0, 100 };
    private int Kasta = -1;
    private int SortBearBy = -1;
    private string BearName = "";
    private int BearIndex = -1;

    private FacilityWindow FacilityWindow = null;
    private int FacilityType = -1;
    private int SortFacilityBy = -1;
    private string FacilityName = "";
    private int FacilityIndex = -1;

    public override string _Label => "База данных города";

    private void OnDestroy()
    {
        City._DataBase.OnBearChanges -= UpdateList;
        City._DataBase.OnFacilityChanges -= UpdateList;
        City._Time.DayChanged -= UpdateList;
    }

    private void Start()
    {
        City._DataBase.OnBearChanges += UpdateList;
        City._Time.DayChanged += UpdateList;
        City._DataBase.OnFacilityChanges += UpdateList;

        UpdateList();

        IlusionHolders.SetInfo(Pointed, Clicked);
    }

    public void EditFindObject()
    {
        UserInteract.AskVariants("", new string[] { "Медведь", "Здание" }, new int[] { 0, 1 }, EditFindObject);
    }
    public void EditFindObject(int bear)
    {
        FindBear = bear == 0;

        BearFilterPage.SetActive(FindBear);
        FacilityFilterPage.SetActive(!FindBear);

        if (FindBear)
        {
            FindObject.text = "Медведь";
        }
        else
        {
            FindObject.text = "Здание";
        }

        UpdateList();
    }

    public void EditBearFilter(int index)
    {
        switch (index)
        {
            case 0:
                UserInteract.AskInput("", SetBearAge0);
                break;
            case 1:
                UserInteract.AskInput("", SetBearAge1);
                break;
            case 2:
                UserInteract.AskVariants("", new string[] { "не важно", "Неопределёно", "Пасечник", "Конструктор", "Программист", "Биоинженер", "Первопроходец", "Творец" }, new int[] { -1, 0, 1, 2, 3, 4, 5, 6 }, SetBearKasta);
                break;
            case 3:
                UserInteract.AskVariants("", new string[] { "По номеру", "По имени", "По навыку", "Не имеет работы", "Не имеет жилья", "По здоровью", "По удовлетворенности", "По возрасту" }, new int[] { -1, 0, 1, 2, 3, 4, 5, 6 }, SetBearSorting);
                break;
            case 4:
                UserInteract.AskInput("", SetBearName);
                break;
        }
    }
    public void SetBearName(string info)
    {
        BearName = info;

        UpdateList();
    }
    public void SetBearIndex(string info)
    {
        if (info.Length <= 0)
        {
            BearIndex = -1;
        }
        else
        {
            BearIndex = StaticTools.StringToInt(info);
        }

        UpdateList();
    }
    public void SetBearAge0(string info)
    {
        BearAge[0] = Mathf.Min(Mathf.Clamp(StaticTools.StringToInt(info), 0, 100), BearAge[1]);
        BearFilters[0].text = $"от {BearAge[0]}";
        UpdateList();
    }
    public void SetBearAge1(string info)
    {
        BearAge[1] = Mathf.Max(Mathf.Clamp(StaticTools.StringToInt(info), 0, 100), BearAge[0]);
        BearFilters[1].text = $"до {BearAge[1]}";
        UpdateList();
    }
    public void SetBearKasta(int index)
    {
        Kasta = index;
        if (Kasta == -1)
        {
            BearFilters[2].text = "Любая";
        }
        else
        {
            BearFilters[2].text = ((Bear.Kasta)index).ToString();
        }

        UpdateList();
    }
    public void SetBearSorting(int index)
    {
        BearNameButton.SetActive(index == 0);
        BearIndexButton.SetActive(index == -1);

        SortBearBy = index;
        BearFilters[3].text = BearSortName(index);

        UpdateList();
    }
    private string BearSortName (int index)
    {
        switch (index)
        {
            case -1:
                return "По номеру";
            case 0:
               return "По имени";
            case 1:
                return "По навыку";
            case 2:
                return "Не имеет работы";
            case 3:
                return "Не имеет жилья";
            case 4:
                return "По здоровью";
            case 5:
                return "По удовлетворенности";
            case 6:
                return "По возрасту";
        }

        return "";
    }

    public void EditFacilityFilter(int index)
    {
        switch (index)
        {
            case 0:
                UserInteract.AskVariants("", new string[] { "Любое", "Жильё", "Электричество", "Промышленность", "Пища", "Развлечение", "Хранилище", "Образование и наука", "Медицина", "Прочее" }, new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8 }, SetFacilityType);
                break;
            case 1:
                UserInteract.AskVariants("", new string[] { "По номеру", "По названию", "По укомплектованности", "По потреблению/производству электричества" }, new int[] { -1, 0, 1, 2 }, SetFacilitySort);
                break;
        }
    }
    public void SetFacilityName(string info)
    {
        FacilityName = info;

        UpdateList();
    }
    public void SetFacilityIndex(string info)
    {
        if (info.Length <= 0)
        {
            FacilityIndex = -1;
        }
        else
        {
            FacilityIndex = StaticTools.StringToInt(info);
        }

        UpdateList();
    }
    public void SetFacilityType(int index)
    {
        FacilityType = index;

        switch (index)
        {
            case -1:
                FacilityFilters[0].text = "Любое";
                break;
            case 0:
                FacilityFilters[0].text = "Жильё";
                break;
            case 1:
                FacilityFilters[0].text = "Электричество";
                break;
            case 2:
                FacilityFilters[0].text = "Промышленность";
                break;
            case 3:
                FacilityFilters[0].text = "Пища";
                break;
            case 4:
                FacilityFilters[0].text = "Развлечение";
                break;
            case 5:
                FacilityFilters[0].text = "Хранилище";
                break;
            case 6:
                FacilityFilters[0].text = "Образование и наука";
                break;
            case 7:
                FacilityFilters[0].text = "Медицина";
                break;
            case 8:
                FacilityFilters[0].text = "Прочее";
                break;
        }

        UpdateList();
    }
    public void SetFacilitySort(int index)
    {
        SortFacilityBy = index;

        FacilityIndexButton.SetActive(index == -1);
        FacilityNameButton.SetActive(index == 0);

        FacilityFilters[1].text = FacilitySortName(index);

        UpdateList();
    }
    public string FacilitySortName(int index)
    {
        switch (index)
        {
            case -1:
                return "По номеру";
            case 0:
                return "По названию";
            case 1:
                return "По укомплектованности";
            case 2:
                return "По электричеству";
        }

        return "";
    }

    public void Pointed(int index)
    {
        if (FindBear)
        {
            if (BearWindow != null)
            {
                BearWindow.SetInfo(Bears[index]);
            }
        }
        else
        {
            if (FacilityWindow != null)
            {
                FacilityWindow.SetInfo(Facilities[index]);
            }
        }
    }
    public void Clicked(int index)
    {
        if (FindBear)
        {
            FindObjectOfType<WindowCreator>().CreateWindow<BearWindow>().SetInfo(Bears[index]);
        }
        else
        {
            FindObjectOfType<WindowCreator>().CreateWindow<FacilityWindow>().SetInfo(Facilities[index]);
        }
    }

    public override void RightMouse()
    {
        UserInteract.AskVariants(_Label, new string[] { $"{(Pinned ? "открепить" : "закрепить")} окно", "закрыть окно", "открыть служебное окно", "изменить параметр поиска" }, new int[] { -1, 0, 1, 2 }, RightMouseActions);
    }
    public override void RightMouseActions(int index)
    {
        switch (index)
        {
            case -1:
                Pinned = !Pinned;
                break;
            case 0:
                Close();
                break;
            case 1:
                if (FindBear)
                {
                    if (BearWindow == null)
                    {
                        BearWindow = FindObjectOfType<WindowCreator>().CreateWindow<BearWindow>();

                        if (Bears.Length > 0)
                        {
                            BearWindow.SetInfo(Bears[0]);
                        }
                    }
                }
                else
                {
                    if (FacilityWindow == null)
                    {
                        FacilityWindow = FindObjectOfType<WindowCreator>().CreateWindow<FacilityWindow>();

                        if (Facilities.Length > 0)
                        {
                            FacilityWindow.SetInfo(Facilities[0]);
                        }
                    }
                }
                break;
            case 2:
                if (FindBear)
                {
                    UserInteract.AskVariants("", new string[] {$"Объект поиска <color=green>Медведь</color>", $"Минимальный возраст <color=green>{BearAge[0]}</color>", $"Максимальный возраст <color=green>{BearAge[1]}</color>", $"Специальность <color=green>{Kasta}</color>", $"Сортировка <color=green>{BearSortName(SortBearBy)}</color>", $"Имя медведя <color=green>{BearName}</color>", $"Номер медведя <color=green>{BearIndex}</color>" }, new int[] {0, 1, 2, 3, 4, 5, 6}, EditBearParameter);
                }
                else
                {
                    UserInteract.AskVariants("", new string[] {$"Объект поиска <color=green>Здание</color>", $"Направление <color=green>{(FacilityType == -1 ? "любое" : (Constructor.ConstructCategory) FacilityType)}</color>", $"Сортировка <color=green>{FacilitySortName(SortFacilityBy)}</color>", $"Название здания <color=green>{FacilityName}</color>", $"Номер здания <color=green>{FacilityIndex}</color>" }, new int[] {0, 1, 2, 3, 4}, EditFacilityParameter);
                }
                break;
        }
    }
    public void EditBearParameter(int index)
    {
        switch (index)
        {
            case 0:
                EditFindObject();
                break;
            case 1:
                EditBearFilter(0);
                break;
            case 2:
                EditBearFilter(1);
                break;
            case 3:
                EditBearFilter(2);
                break;
            case 4:
                EditBearFilter(3);
                break;
            case 5:
                UserInteract.AskInput("", SetBearName);
                break;
            case 6:
                UserInteract.AskInput("", SetBearIndex);
                break;
        }
    }
    public void EditFacilityParameter(int index)
    {
        switch (index)
        {
            case 0:
                EditFindObject();
                break;
            case 1:
                EditFacilityFilter(0);
                break;
            case 2:
                EditFacilityFilter(1);
                break;
            case 3:
                UserInteract.AskInput("", SetFacilityName);
                break;
            case 4:
                UserInteract.AskInput("", SetFacilityIndex);
                break;
        }
    }

    public void UpdateList()
    {
        if (FindBear)
        {
            Bear[] bears = new Bear[0];

            if (SortBearBy == -1 && BearIndex != -1 && !(BearIndex < 0 || BearIndex >= City._DataBase._Bears.Length))
            {
                bears = new Bear[] { City._DataBase._Bears[BearIndex] };
            }
            else
            {
                foreach (Bear bear in City._DataBase._Bears)
                {
                    if (bear._Age >= BearAge[0] && bear._Age <= BearAge[1])
                    {
                        if (Kasta == -1 || bear._Kasta.GetHashCode() == Kasta)
                        {
                            bears = StaticTools.ExpandMassive(bears, bear);
                        }
                    }
                }

                int[] bearSatisfactions = new int[0];
                int[] bearNamePriorities = new int[0];
                if (SortBearBy == 5)
                {
                    bearSatisfactions = new int[bears.Length];
                    for (int i = 0; i < bears.Length; i++)
                    {
                        bearSatisfactions[i] = bears[i]._Satisfaction;
                    }
                }
                else if(SortBearBy == 0 && BearName.Length > 0)
                {
                    bearNamePriorities = new int[bears.Length];
                    for (int i = 0; i < bears.Length; i++)
                    {
                        bearNamePriorities[i] = StaticTools.Match(BearName, bears[i]._Name);
                    }
                }

                for (int i = 0; i < bears.Length; i++)
                {
                    int high = i;

                    for (int ii = i + 1; ii < bears.Length; ii++)
                    {
                        switch (SortBearBy)
                        {
                            case 0:
                                if (BearName.Length <= 0)
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
                            case 1:
                                if (bears[ii]._Skill > bears[high]._Skill)
                                {
                                    high = ii;
                                }
                                break;
                            case 2:
                                if ((bears[ii]._Facility == null).GetHashCode() > (bears[high]._Facility == null).GetHashCode())
                                {
                                    high = ii;
                                }
                                break;
                            case 3:
                                if ((bears[ii]._Facility == null).GetHashCode() > (bears[high]._Facility == null).GetHashCode())
                                {
                                    high = ii;
                                }
                                break;
                            case 4:
                                if (bears[ii]._Health < bears[high]._Health)
                                {
                                    high = ii;
                                }
                                break;
                            case 5:
                                if (bearSatisfactions[ii] < bearSatisfactions[high])
                                {
                                    high = ii;
                                }
                                break;
                            case 6:
                                if (bears[ii]._Age < bears[high]._Age)
                                {
                                    high = ii;
                                }
                                break;
                        }
                    }

                    Bear buffer = bears[i];
                    bears[i] = bears[high];
                    bears[high] = buffer;

                    if (SortBearBy == 5)
                    {
                        int sbuffer = bearSatisfactions[i];
                        bearSatisfactions[i] = bearSatisfactions[high];
                        bearSatisfactions[high] = sbuffer;
                    }
                    else if (SortBearBy == 0 && BearName.Length > 0)
                    {
                        int sbuffer = bearNamePriorities[i];
                        bearNamePriorities[i] = bearNamePriorities[high];
                        bearNamePriorities[high] = sbuffer;
                    }
                }
            }

            Bears = bears;

            string info = "";

            for (int i = 0; i < bears.Length; i++)
            {
                switch (SortBearBy)
                {
                    case -1:
                        info += $"{bears[i]._Name}  #{StaticTools.IndexOf(City._DataBase._Bears, bears[i])}\n";
                        break;
                    case 1:
                        info += $"{bears[i]._Name}  {bears[i]._Skill}\n";
                        break;
                    case 2:
                        info += $"{bears[i]._Name}  {(bears[i]._Facility != null ? bears[i]._Facility._ConstructInfo.Name + $"#{StaticTools.IndexOf(City._DataBase._Facilities, bears[i]._Facility)}" : "нет")}\n";
                        break;
                    case 3:
                        info += $"{bears[i]._Name}  {(bears[i]._Home != null ? bears[i]._Home._ConstructInfo.Name + $"#{StaticTools.IndexOf(City._DataBase._Facilities, bears[i]._Home)}" : "нет")}\n";
                        break;
                    case 4:
                        info += $"{bears[i]._Name}  {bears[i]._Health}\n";
                        break;
                    case 5:
                        info += $"{bears[i]._Name}  {bears[i]._Satisfaction}\n";
                        break;
                    case 6:
                        info += $"{bears[i]._Name}  {bears[i]._Age}\n";
                        break;
                    default:
                        info += $"{bears[i]._Name}\n";
                        break;
                }
            }

            List.text = info;
            IlusionHolders._MaxIndex = bears.Length -1;

            Content.sizeDelta = new Vector2(0, 35 * Bears.Length);

            if (bears.Length > 0 &&BearWindow != null)
            {
                BearWindow.SetInfo(bears[0]);
            }
        }
        else
        {
            Facility[] facilities = new Facility[0];

            if (SortFacilityBy == -1 && FacilityIndex != -1 && !(FacilityIndex < 0 || FacilityIndex >= City._DataBase._Facilities.Length))
            {
                facilities = new Facility[] { City._DataBase._Facilities[FacilityIndex] };
            }
            else
            {
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    switch (FacilityType)
                    {
                        default:
                            facilities = StaticTools.ExpandMassive(facilities, facility);
                            break;
                        case 0:
                            if (facility is Home)
                            {
                                facilities = StaticTools.ExpandMassive(facilities, facility);
                            }
                            break;
                        case 1:
                            if (facility is EnergyProcuder)
                            {
                                facilities = StaticTools.ExpandMassive(facilities, facility);
                            }
                            break;
                        case 3:
                            if (facility is FoodProducer)
                            {
                                facilities = StaticTools.ExpandMassive(facilities, facility);
                            }
                            break;
                    }
                }

                for (int i = 0; i < facilities.Length; i++)
                {
                    int high = i;
                    for (int ii = i + 1; ii < facilities.Length; ii++)
                    {
                        switch (SortFacilityBy)
                        {
                            case 0:
                                if (FacilityName.Length <= 0)
                                {
                                    if (facilities[ii]._ConstructInfo.Name.CompareTo(facilities[high]._ConstructInfo.Name) < 0)
                                    {
                                        high = ii;
                                    }
                                }
                                else
                                {
                                    if (StaticTools.Match(FacilityName, facilities[ii]._ConstructInfo.Name) > StaticTools.Match(FacilityName, facilities[high]._ConstructInfo.Name))
                                    {
                                        high = ii;
                                    }
                                }
                                break;
                            case 1:
                                if (facilities[ii]._MaxBearCount - facilities[ii]._AssignedBears.Length > facilities[high]._MaxBearCount - facilities[high]._AssignedBears.Length)
                                {
                                    high = ii;
                                }
                                break;
                            case 2:
                                if (((facilities[ii] is EnergyProcuder) ? -(facilities[ii] as EnergyProcuder)._Produce : facilities[ii]._EnergyConsume) < ((facilities[ii] is EnergyProcuder) ? -(facilities[ii] as EnergyProcuder)._Produce : facilities[ii]._EnergyConsume))
                                {
                                    high = ii;
                                }
                                break;
                        }
                    }

                    Facility buffer = facilities[i];
                    facilities[i] = facilities[high];
                    facilities[high] = buffer;
                }
            }

            Facilities = facilities;

            string info = "";

            for (int i = 0; i < facilities.Length; i++)
            {
                switch (SortFacilityBy)
                {
                    default:
                        info += $"{facilities[i]._ConstructInfo.Name}\n";
                        break;
                    case -1:
                        info += $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])}\n";
                        break;
                    case 1:
                        info += $"{facilities[i]._ConstructInfo.Name} {facilities[i]._AssignedBears.Length}/{facilities[i]._MaxBearCount}\n";
                        break;
                    case 2:
                        info += $"{facilities[i]._ConstructInfo.Name} {(facilities[i] is EnergyProcuder ? $"+{(facilities[i] as EnergyProcuder)._Produce}" : $"-{facilities[i]._EnergyConsume}")}\n";
                        break;
                }
            }

            List.text = info;
            IlusionHolders._MaxIndex = facilities.Length-1;

            Content.sizeDelta = new Vector2(0, 35 * Facilities.Length);

            if (facilities.Length > 0 && FacilityWindow != null)
            {
                FacilityWindow.SetInfo(facilities[0]);
            }
        }
    }
}
