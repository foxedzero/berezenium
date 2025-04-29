using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BearWindow : DefaultWindow
{
    [SerializeField] private Image HeadIcon;
    [SerializeField] private Image FaceIcon;
    [SerializeField] private Image BrowsIcon;

    [SerializeField] private Text Name;

    [SerializeField] private Text HpText;
    [SerializeField] private Text StressText;
    [SerializeField] private Text TiredText;
    [SerializeField] private Text WorkText;

    [SerializeField] private Text SpecializationText;
    [SerializeField] private Text AddictionalText;
    [SerializeField] private Text HomeText;
    [SerializeField] private Text FacilityText;
    [SerializeField] private Text ColdText;
    [SerializeField] private Text ScheduleText;
    [SerializeField] private Bear Bear;

    [SerializeField] private Image HpBar;
    [SerializeField] private Image StressBar;
    [SerializeField] private Image HeatBar;
    [SerializeField] private Image TiredBar;
    [SerializeField] private Image WorkBar;
    [SerializeField] private Tipper ColdTip;
    [SerializeField] private Tipper StressTip;
    [SerializeField] private Tipper WorkTip;

    public override string _Label => $"информация о {Bear._Name}";

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(Bear != null)
        {
            Bear.OnSmthChange -= UpdateInfo;
        }
    }

    public void SetInfo(Bear bear)
    {
        if (Bear != null)
        {
            Bear.OnSmthChange -= UpdateInfo;
        }

        Bear = bear;

        foreach(Window window in Lister._Windows)
        {
            if(window is BearWindow && this != window)
            {
                if((window as BearWindow).Bear == Bear)
                {
                    window.Close();
                    break;
                }
            }
        }

        Bear.OnSmthChange += UpdateInfo;

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        Name.text = Bear._Name;
        HpText.text = $"Здоровье: {Bear._Health}/10";

        StressText.text = $"Стресс: {(int)Bear._Stress}%";
        TiredText.text = $"Усталость: {(int)(Bear._Tired * 100)}%";
        WorkText.text = $"Работа: {(int)(Bear._Work * 100)}%";

        AddictionalText.text = $"Возраст: {Bear._Age}";
        SpecializationText.text = $"Специализация: {Bear._Kasta}";
        FacilityText.text = $"Работа: {(Bear._Facility != null ? $"{Bear._Facility._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, Bear._Facility)}" : "отсутствует")}";
        HomeText.text = $"Проживание: {(Bear._Home != null ? $"{Bear._Home._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, Bear._Home)}" : "отсутствует")}";

        ScheduleText.text = $"Распорядок: #{Bear._Schedule}";

        HpBar.fillAmount = Bear._Health / 10f;
        StressBar.fillAmount = Bear._Stress / 100f;
        HeatBar.fillAmount = Bear._HeatLevel / 10f + 0.5f;
        TiredBar.fillAmount = Bear._Tired;
        WorkBar.fillAmount = Bear._Work;

        ColdText.text = $"Уровень тепла: {(Bear._HeatLevel > 0 ? "+" : "")}{Bear._HeatLevel}";

        string info = $"Стресс медведя.\n\nСтресс - это естественная реакция организма на недостаточно комфортные условия. Если он будет критически высоким, то медведь перестанет работать. Если общий уровень стресса всех медведей достигнет максимума, это будет означать конец ваших капитанских полномочий.\n\nФакторы стресса ед/сут:";
        foreach (CityFactors.Factor factor in Bear.GetStressFactors())
        {
            info += $"\n{factor}";
        }
        StressTip._Info = info ;

        ColdTip._Info = $"Уровень тепла.\n\nЕсли температура будет ниже нуля, то медведь начнёт терять здоровье." +
            $"\n\nХладостойкость от сытости: {Mathf.RoundToInt(2 * City._Foodstream._Saturation)}" +
            $"\nХладостойкость помещения: {(Bear._CurrentFacility != null ? Bear._CurrentFacility._ColdEndurance : 0)}" +
            $"\nУровень тепла окружающей среды: {-City._Weather._Cold}" +
            $"\nИтоговый уровень тепла: {Bear._HeatLevel}";

        info = $"Работоспособность медведя на работе.\n\nУчтите, что медведь, работающий не по специальности, в два раза менее эффективен на занимаемом им посту.\n\nФакторы работоспособности:";
        foreach (CityFactors.Factor factor in Bear.GetWorkFactors())
        {
            info += $"\n{factor}";
        }

        WorkTip._Info = info;

        BearLook bearLook = null;
        if (Bear is SuperBear)
        {
            bearLook = City._BearObjectioner.GetBearIcon(Bear as SuperBear);
        }
        else
        {
            bearLook = City._BearObjectioner.GetBearIcon(Bear._Face, Bear._Brows, Bear._BodyColor);
        }

        if (bearLook.Face == null)
        {
            HeadIcon.sprite = bearLook.Head;
            HeadIcon.color = new Color(1, 1, 1, 1);
            FaceIcon.color = new Color(0, 0, 0, 0);
            BrowsIcon.color = new Color(0, 0, 0, 0);
        }
        else
        {
            HeadIcon.sprite = bearLook.Head;
            HeadIcon.color = bearLook.SkinColor;
            FaceIcon.sprite = bearLook.Face;
            BrowsIcon.sprite = bearLook.Brows;
            FaceIcon.color = new Color(1, 1, 1, 1);
            BrowsIcon.color = new Color(1, 1, 1, 1);
        }
    }

    public void SelectWork()
    {
        if(Bear._Facility != null)
        {
            UserInteract.AskVariants("", new string[] {"Информация", "Снять с назначения", "Переназначить"}, new int[] {0, 1, 2}, RightMouseWork);

        }
        else
        {
            RightMouseWork(2);
        }
    }

    public void SelectSchedule()
    {
        string[] variants = new string[City._CitySchedule._Schedules.Length];
        int[] indexes = new int[variants.Length];
        for (int i = 0; i < variants.Length; i++)
        {
            variants[i] = $"Расписание #{City._CitySchedule._Schedules[i]._Index}";
            indexes[i] = i;
        }

        UserInteract.AskVariants("Назначить", variants, indexes, SelectSchedule);
    }
    public void SelectSchedule(int index)
    {
        City._CitySchedule._Schedules[Bear._Schedule].RegisterBear(Bear, true);
        Bear._Schedule = index;
        City._CitySchedule._Schedules[Bear._Schedule].RegisterBear(Bear, false);
    }

    public void RightMouseWork(int index)
    {
        switch (index)
        {
            case 0:
                WindowCreator.CreateWindow<FacilityWindow>().SetInfo(Bear._Facility);
                break;
            case 1:
                Bear._Facility.AssignBear(Bear, true);
                break;
            case 2:
                UserInteract.AskFacility("Назначить работу", SetFacility, UserFacility.Sorting.Kasta, Bear._Kasta.ToString(), exlude: Bear._Facility != null ? new int[] {StaticTools.IndexOf(City._DataBase._Facilities, Bear._Facility)} : null);
                break;
        }
    }
    public void SetFacility(Facility facility)
    {
        facility.AssignBear(Bear, false);
    }

    public void SelectHome()
    {
        if (Bear._Home != null)
        {
            UserInteract.AskVariants("", new string[] { "Информация", "Снять с назначения", "Переназначить" }, new int[] { 0, 1, 2 }, RightMouseHome);

        }
        else
        {
            RightMouseHome(2);
        }
    }
    public void RightMouseHome(int index)
    {
        switch (index)
        {
            case 0:
                WindowCreator.CreateWindow<FacilityWindow>().SetInfo(Bear._Home);
                break;
            case 1:
                Bear._Home.AssignBear(Bear, true);
                break;
            case 2:
                UserInteract.AskFacility("Назначить жилище", SetFacility, UserFacility.Sorting.Type, Constructor.ConstructCategory.Жилище.ToString(), exlude: Bear._Home != null ? new int[] { StaticTools.IndexOf(City._DataBase._Facilities, Bear._Home) } : null);
                break;
        }
    }
}
