using UnityEngine;
using UnityEngine.UI;
using WindowInterfaces;

public class ResearchWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private Text Category;
    [SerializeField] private Text Describtion;
    [SerializeField] private Text List;
    [SerializeField] private Text Level;

    [SerializeField] private Image LevelFill;
    [SerializeField] private Tipper LevelTip;

    [SerializeField] private RectTransform Content;
    [SerializeField] private IlusionHolders IlusionHolders;

    private Facility[] Facilities = new Facility[0];

    private static CityResearch.ResearchType ResearchType = CityResearch.ResearchType.Electricity;

    public override string _Label => $"Исследования";

    private void OnDestroy()
    {
        City._Research.OnReseatchUpdate -= UpdateInfo;
    }

    private void Start()
    {
        IlusionHolders.SetInfo(null, Click);
        UpdateInfo();

        City._Research.OnReseatchUpdate += UpdateInfo;
    }

    public void SetCategory()
    {
        UserInteract.AskVariants("Направление", new string[] { "Электроэнергия", "Отопительные системы", "Медицина и препараты", "Путешествия и логистика", "Жилищные условия", "Пасеки и мёд", "Производство", "Добыча" }, new int[] { 0,1,2,3,4,5, 6, 7 }, SetCategory);
    }
    public void SetCategory(int index)
    {
        ResearchType = (CityResearch.ResearchType)index;

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        Facilities = new Facility[0];

        foreach(Facility facility in City._DataBase._Facilities)
        {
            if(facility is Laboratory)
            {
                if((facility as Laboratory)._Target == ResearchType)
                {
                    Facilities = StaticTools.ExpandMassive(Facilities, facility);
                }
            }
        }

        switch (ResearchType)
        {
            case CityResearch.ResearchType.Electricity:
                Category.text = $"Электроэнергия";
                Describtion.text = $"Данное направление позволит строить новые электростанции, строить аккумуляторы, а также увеличить КПД электросистемы.";
                break;
            case CityResearch.ResearchType.Cold:
                Category.text = $"Отопительные системы";
                Describtion.text = $"Данное направление позволит разработать новые методы отопления.";
                break;
            case CityResearch.ResearchType.Medicine:
                Category.text = $"Медицина и препараты";
                Describtion.text = $"Данное направление позволит улучшить качество медицины и делать новые препараты.";
                break;
            case CityResearch.ResearchType.Travels:
                Category.text = $"Путешествия и логистика";
                Describtion.text = $"Данное направление позволит строить снегоходы, космопорты.";
                break;
            case CityResearch.ResearchType.Household:
                Category.text = $"Жилищные условияа";
                Describtion.text = $"Данное направление позволит строить вместительные и защищенные дома.";
                break;
            case CityResearch.ResearchType.Food:
                Category.text = $"Пасеки и мёд";
                Describtion.text = $"Данное направление позволит увеличить стойкость к морозам и производство.";
                break;
            case CityResearch.ResearchType.Production:
                Category.text = $"Производство";
                Describtion.text = $"Данное направление позволит усовершенствовать заводы, строить новые виды заводов, а также уменьшить производственные издержки.";
                break;
            case CityResearch.ResearchType.Mining:
                Category.text = $"Добыча";
                Describtion.text = $"Данное направление позволит больше и быстрее добывать ресурсы с месторождений, а также научиться добывать березениум.";
                break;
        }

        string info = "Назначить лабораторию";
        foreach(Facility facility1 in Facilities)
        {
            info += $"\n{facility1._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facility1)}";

        }

        List.text = info;
        IlusionHolders._MaxIndex = Facilities.Length;

        Content.sizeDelta = new Vector2(0, 35 * (Facilities.Length + 1));

        if(City._Research.GetResearchGoal(ResearchType) == -1)
        {
            info = $"Максимальный прогресс";
        }
        else
        {
            info = $"До следующего уровня: {City._Research.GetResearchGoal(ResearchType) - City._Research.GetResearch(ResearchType)}";
        }
        LevelFill.fillAmount = City._Research.GetResearch(ResearchType) / City._Research.GetResearchGoal(ResearchType);


        switch (ResearchType)
        {
            case CityResearch.ResearchType.Electricity:
                info += $"\n\n1 Уровень: Аккумулятор\n\n2 Уровень: Увеличение КПД\n\n3 Уровень: Березениумные электростанции";
                break;
            case CityResearch.ResearchType.Cold:
                info += $"\n\n1 Уровень: Обогреватели\n\n2 Уровень: Уличные отопители (не реализован еще)\n\n3 Уровень: Центральное отопление (не реализован еще)";
                break;
            case CityResearch.ResearchType.Medicine:
                info += $"\n\n1 Уровень: Препараты против спячки (не реализован еще)\n\n2 Уровень: Увеличение качества лечения\n\n3 Уровень: Стимуляторы (не реализован еще)";
                break;
            case CityResearch.ResearchType.Travels:
                info += $"\n\n1 Уровень: Утеплённое снаряжение разведчиков\n\n2 Уровень: Снегоходы разведчикам\n\n3 Уровень: Космолёт";
                break;
            case CityResearch.ResearchType.Household:
                info += $"\n\n1 Уровень: Многоэтажка\n\n2 Уровень: Повышение хлодостойкости\n\n3 Уровень: Особняк (не реализован еще)";
                break;
            case CityResearch.ResearchType.Food:
                info += $"\n\n1 Уровень: Увеличение хладостойкости\n\n2 Уровень: Увеличение производительности\n\n3 Уровень: Киберпасека (не реализован еще)";
                break;
            case CityResearch.ResearchType.Production:
                info += $"\n\n1 Уровень: Завод энергомёда\n\n2 Уровень: Уменьшение затрат\n\n3 Уровень: Завод двойной обработки березениума (не реализован еще)";
                break;
            case CityResearch.ResearchType.Mining:
                info += $"\n\n1 Уровень: Увеличение хладостойкости и добыча березениума\n\n2 Уровень: Увеличение количества и скорости добычи\n\n3 Уровень: Сканер месторождений (не реализован еще)";
                break;
        }

        Level.text = $"Уровень: {City._Research.GetResearchLevel(ResearchType)}";
        LevelTip._Info = info;
    }

    public void Click(int index)
    {
        if(index == 0)
        {
            Facility[] facilities = GetLabolatories();
            string[] variants = new string[facilities.Length];

            for(int i = 0; i < facilities.Length; i++)
            {
                switch((facilities[i] as Laboratory)._Target)
                {
                    case CityResearch.ResearchType.Electricity:
                        variants[i] = $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])} (Электроэнергия)";
                        break;
                    case CityResearch.ResearchType.Cold:
                        variants[i] = $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])} (Отопительные системы)";
                        break;
                    case CityResearch.ResearchType.Medicine:
                        variants[i] = $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])} (Медицина и препараты)";
                        break;
                    case CityResearch.ResearchType.Travels:
                        variants[i] = $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])} (Путешествия и логистика)";
                        break;
                    case CityResearch.ResearchType.Household:
                        variants[i] = $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])} (Жилищные условия)";
                        break;
                    case CityResearch.ResearchType.Food:
                        variants[i] = $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])} (Пасеки и мёд)";
                        break;
                    case CityResearch.ResearchType.Production:
                        variants[i] = $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])} (Производство)";
                        break;
                    case CityResearch.ResearchType.Mining:
                        variants[i] = $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])} (Добыча)";
                        break;
                }
            }

            if(variants.Length > 0)
            {
                UserInteract.AskVariants("", variants, StaticTools.Range(variants.Length), Assign);
            }
        }
        else
        {
            
        }
    }

    public void Assign(int index)
    {
        (GetLabolatories()[index] as Laboratory)._Target = ResearchType;
    }

    private Facility[] GetLabolatories()
    {
        Facility[] facilities = new Facility[0];

        foreach (Facility facility in City._DataBase._Facilities)
        {
            if (facility is Laboratory)
            {
                if ((facility as Laboratory)._Target != ResearchType)
                {
                    facilities = StaticTools.ExpandMassive(facilities, facility);
                }
            }
        }

        return facilities;
    }
}
