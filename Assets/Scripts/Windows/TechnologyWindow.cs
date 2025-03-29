using UnityEngine;
using UnityEngine.UI;
using WindowInterfaces;

public class TechnologyWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private Tipper[] EnergosystemTip;
    [SerializeField] private Tipper[] ColdTip;
    [SerializeField] private Tipper[] MedicineTip;
    [SerializeField] private Tipper[] TravelTip;
    [SerializeField] private Tipper[] HouseholdTip;
    [SerializeField] private Tipper[] FoodTip;
    [SerializeField] private Tipper[] ProductionTip;
    [SerializeField] private Tipper[] MiningTip;

    [SerializeField] private Image[] EnergosystemFill;
    [SerializeField] private Image[] ColdFill;
    [SerializeField] private Image[] MedicineFill;
    [SerializeField] private Image[] TravelFill;
    [SerializeField] private Image[] HouseholdFill;
    [SerializeField] private Image[] FoodFill;
    [SerializeField] private Image[] ProductionFill;
    [SerializeField] private Image[] MiningFill;

    [SerializeField] [Multiline] private string[] EnergosystemDescribtion;
    [SerializeField][Multiline] private string[] ColdDescribtion;
    [SerializeField][Multiline] private string[] MedicineDescribtion;
    [SerializeField][Multiline] private string[] TravelDescribtion;
    [SerializeField][Multiline] private string[] HouseholdDescribtion;
    [SerializeField][Multiline] private string[] FoodDescribtion;
    [SerializeField][Multiline] private string[] ProductionDescribtion;
    [SerializeField][Multiline] private string[] MiningDescribtion;

    private CityResearch.ResearchType ResearchType = CityResearch.ResearchType.Electricity;

    public override string _Label => "Окно технологий";

    private void Start()
    {
        City._Time.HourPassed += HourPassed;
        HourPassed();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        City._Time.HourPassed -= HourPassed;
    }

    public void HourPassed()
    {
        EnergosystemTip[0]._Info = $"{EnergosystemDescribtion[0]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Electricity)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Electricity, 0))}%";
        EnergosystemTip[1]._Info = $"{EnergosystemDescribtion[1]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Electricity)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Electricity, 1))}%";
        EnergosystemTip[2]._Info = $"{EnergosystemDescribtion[2]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Electricity)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Electricity, 2))}%";
        EnergosystemFill[0].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Electricity, 0);
        EnergosystemFill[1].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Electricity, 1);
        EnergosystemFill[2].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Electricity, 2);

        ColdTip[0]._Info = $"{ColdDescribtion[0]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Cold)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Cold, 0))}%";
        ColdTip[1]._Info = $"{ColdDescribtion[1]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Cold)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Cold, 1))}%";
        ColdTip[2]._Info = $"{ColdDescribtion[2]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Cold)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Cold, 2))}%";
        ColdFill[0].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Cold, 0);
        ColdFill[1].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Cold, 1);
        ColdFill[2].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Cold, 2);

        MedicineTip[0]._Info = $"{MedicineDescribtion[0]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Medicine)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Medicine, 0))}%";
        MedicineTip[1]._Info = $"{MedicineDescribtion[1]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Medicine)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Medicine, 1))}%";
        MedicineTip[2]._Info = $"{MedicineDescribtion[2]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Medicine)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Medicine, 2))}%";
        MedicineFill[0].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Medicine, 0);
        MedicineFill[1].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Medicine, 1);
        MedicineFill[2].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Medicine, 2);

        TravelTip[0]._Info = $"{TravelDescribtion[0]}\n\nТребуемая специализация:  {City._Research.ResearchKasta(CityResearch.ResearchType.Travels)} \nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Travels, 0))}%";
        TravelTip[1]._Info = $"{TravelDescribtion[1]}\n\nТребуемая специализация:  {City._Research.ResearchKasta(CityResearch.ResearchType.Travels)} \nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Travels, 1))}%";
        TravelTip[2]._Info = $"{TravelDescribtion[2]}\n\nТребуемая специализация:  {City._Research.ResearchKasta(CityResearch.ResearchType.Travels)} \nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Travels, 2))}%";
        TravelFill[0].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Travels, 0);
        TravelFill[1].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Travels, 1);
        TravelFill[2].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Travels, 2);

        HouseholdTip[0]._Info = $"{HouseholdDescribtion[0]}\n\nТребуемая специализация:  {City._Research.ResearchKasta(CityResearch.ResearchType.Household)} \nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Household, 0))}%";
        HouseholdTip[1]._Info = $"{HouseholdDescribtion[1]}\n\nТребуемая специализация:  {City._Research.ResearchKasta(CityResearch.ResearchType.Household)} \nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Household, 1))}%";
        HouseholdTip[2]._Info = $"{HouseholdDescribtion[2]}\n\nТребуемая специализация:  {City._Research.ResearchKasta(CityResearch.ResearchType.Household)} \nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Household, 2))}%";
        HouseholdFill[0].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Household, 0);
        HouseholdFill[1].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Household, 1);
        HouseholdFill[2].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Household, 2);

        FoodTip[0]._Info = $"{FoodDescribtion[0]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Food)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Food, 0))}%";
        FoodTip[1]._Info = $"{FoodDescribtion[1]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Food)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Food, 1))}%";
        FoodTip[2]._Info = $"{FoodDescribtion[2]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Food)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Food, 2))}%";
        FoodFill[0].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Food, 0);
        FoodFill[1].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Food, 1);
        FoodFill[2].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Food, 2);

        ProductionTip[0]._Info = $"{ProductionDescribtion[0]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Production)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Production, 0))}%";
        ProductionTip[1]._Info = $"{ProductionDescribtion[1]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Production)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Production, 1))}%";
        ProductionTip[2]._Info = $"{ProductionDescribtion[2]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Production)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Production, 2))}%";
        ProductionFill[0].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Production, 0);
        ProductionFill[1].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Production, 1);
        ProductionFill[2].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Production, 2);

        MiningTip[0]._Info = $"{MiningDescribtion[0]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Mining)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Mining, 0))}%";
        MiningTip[1]._Info = $"{MiningDescribtion[1]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Mining)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Mining, 1))}%";
        MiningTip[2]._Info = $"{MiningDescribtion[2]}\n\nТребуемая специализация: {City._Research.ResearchKasta(CityResearch.ResearchType.Mining)}\nИзучено {(int)(100 * City._Research.GetResearchProgress(CityResearch.ResearchType.Mining, 2))}%";
        MiningFill[0].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Mining, 0);
        MiningFill[1].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Mining, 1);
        MiningFill[2].fillAmount = City._Research.GetResearchProgress(CityResearch.ResearchType.Mining, 2);
    }

    public string ResearchName(CityResearch.ResearchType type)
    {
        switch (type)
        {
            case CityResearch.ResearchType.Electricity:
                return "Электроэнергия";
            case CityResearch.ResearchType.Cold:
                return "Отопление";
            case CityResearch.ResearchType.Medicine:
                return "Медицина";
            case CityResearch.ResearchType.Travels:
                return "Путешествия";
            case CityResearch.ResearchType.Household:
                return "Жилищные условия";
            case CityResearch.ResearchType.Food:
                return "Пища";
            case CityResearch.ResearchType.Production:
                return "Производство";
            case CityResearch.ResearchType.Mining:
                return "Добыча";
        }
        return "";
    }

    public void ShowLaboratories(bool set)
    {
        Laboratory[] laboratories = new Laboratory[0];
        int[] labIndexes = new int[0];

        for(int i =0; i< City._DataBase._Facilities.Length; i++)
        {
            if(City._DataBase._Facilities[i] is Laboratory)
            {
                labIndexes = StaticTools.ExpandMassive(labIndexes, i);
                laboratories = StaticTools.ExpandMassive(laboratories, City._DataBase._Facilities[i] as Laboratory);
            }
        }

        if(laboratories.Length <= 0)
        {
            UserInteract.AskMessage("Нет лабораторий", "У вас нет лабораторий, возможно стоит их построить.");
            return;
        }

        string[] variants = new string[laboratories.Length];
        int[] indexes = new int[laboratories.Length];
        
        for(int i = 0; i < laboratories.Length; i++)
        {
            variants[i] = $"{laboratories[i]._ConstructInfo.Name} #{labIndexes[i]}: {ResearchName(laboratories[i]._Target)}";
            indexes[i] = i ;
        }

        if (set)
        {
            UserInteract.AskVariants($"Назначить лабораторию на {ResearchName(ResearchType)}", variants, indexes, SetLaboratory);
        }
        else
        {
            UserInteract.AskVariants("Лаборатории", variants, indexes, SelectLaboratory);
        }
    }
    public void SelectLaboratory(int index)
    {
        Laboratory[] laboratories = new Laboratory[0];

        for (int i = 0; i < City._DataBase._Facilities.Length; i++)
        {
            if (City._DataBase._Facilities[i] is Laboratory)
            {
                laboratories = StaticTools.ExpandMassive(laboratories, City._DataBase._Facilities[i] as Laboratory);
            }
        }

        WindowCreator.CreateWindow<FacilityWindow>().SetInfo(laboratories[index]);
    }

    public void AssignLaboratories(int type)
    {
        ResearchType = (CityResearch.ResearchType)type;
        ShowLaboratories(true);
    }

    public void SetLaboratory(int index)
    {
        Laboratory[] laboratories = new Laboratory[0];

        for (int i = 0; i < City._DataBase._Facilities.Length; i++)
        {
            if (City._DataBase._Facilities[i] is Laboratory)
            {
                laboratories = StaticTools.ExpandMassive(laboratories, City._DataBase._Facilities[i] as Laboratory);
            }
        }

        laboratories[index]._Target = ResearchType;
    }
}
