using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BearWindow : DefaultWindow
{
    [SerializeField] private Image Icon;
    [SerializeField] private Text Name;
    [SerializeField] private Text HpText;
    [SerializeField] private Text SatisText;
    [SerializeField] private Text SkillText;
    [SerializeField] private Text SpecializationText;
    [SerializeField] private Text AddictionalText;
    [SerializeField] private Text HomeText;
    [SerializeField] private Text WorkText;
    [SerializeField] private Text ColdText;
    [SerializeField] private Bear Bear;
    [SerializeField] private Image HpBar;
    [SerializeField] private Image SatisBar;
    [SerializeField] private Image SkillBar;
    [SerializeField] private Tipper SatisfactionTip;
    [SerializeField] private Tipper SkillTip;
    [SerializeField] private Tipper ColdTip;

    public override string _Label => $"информация о {Bear._Name}";

    private void OnDestroy()
    {
        if(Bear != null)
        {
            Bear.OnSmthChange -= UpdateInfo;
        }

        City._Time.DayChanged -= UpdateInfo;
    }

    private void Start()
    {
        City._Time.DayChanged += UpdateInfo;
    }

    public void SetInfo(Bear bear)
    {
        if (Bear != null)
        {
            Bear.OnSmthChange -= UpdateInfo;
        }

        Bear = bear;

        Bear.OnSmthChange += UpdateInfo;

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        Name.text = Bear._Name;
        HpText.text = $"Здоровье: {Bear._Health}/10";
        SatisText.text = $"Счастье: {Bear._Satisfaction}/10";
        SkillText.text = $"Опыт: {Bear._Skill}";

        AddictionalText.text = $"Возраст: {Bear._Age}      Пол: {(Bear._Woman ? "Женский" : "Мужской")}";
        SpecializationText.text = $"Специализация: {Bear._Kasta}";
        WorkText.text = $"Работа: {(Bear._Facility != null ? $"{Bear._Facility._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, Bear._Facility)}" : "отсутствует")}";
        HomeText.text = $"Проживание: {(Bear._Home != null ? $"{Bear._Home._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, Bear._Home)}" : "отсутствует")}";

        HpBar.fillAmount = Bear._Health / 10f;
        SatisBar.fillAmount = Bear._Satisfaction / 10f;
        SkillBar.fillAmount = Bear._Skill / 999f;

        int cold = Bear._ColdEndurance - City._Weather._Cold;
      
        if(cold < -2)
        {
            ColdText.text = $"<color=red>{cold}</color>";
        }
        else
        {
            ColdText.text = $"{cold}";
        }

        ColdTip._Info = $"Уровень тепла.\n\nМедведи сами по себе неплохо приспособлены к холоду, но при уровне тепла ниже -2, начнут терять здоровье и жаловаться на холод." +
            $"\nЕсли уровень упадет ниже -5, то медведь будет терять здоровье в два раза больше." +
            $"\n\nХладостойкость дома: {(Bear._Home != null ? Bear._Home._ColdEndurance : 0)}" +
            $"\nХладостойкость места работы: {(Bear._Facility != null ? Bear._Facility._ColdEndurance : 0)}" +
            $"\nИтоговая хладостойкость: {Bear._ColdEndurance}" +
            $"\nУровень тепла окружающей среды: {-City._Weather._Cold}";

        string info = $"Удовлетворённость медведя, за ней стоит следить.\n\nЕсли все медведи в городе будут недовольны, то краток путь вашего капитанства.\n\nФакторы счастья:";
        foreach(CityFactors.Factor factor in Bear.GetSatisfactionFactors())
        {
            info += $"\n{factor}";
        }

        SatisfactionTip._Info = info;

        info = $"Значение опыта медведя определяет его эффективность на работе.\n\nСтоит понимать, что медведь работающий не по специальности получает штраф эффективности - 0.25x.\n\nБазовый опыт растёт на работе.\n\nБазовый опыт не сможет превысить 999 ед.\n\nФакторы опыта:";
        foreach (CityFactors.Factor factor in Bear.GetSkillFactors())
        {
            info += $"\n{factor}";
        }

        SkillTip._Info = info;
    }

    public void SelectWork()
    {
        if(Bear._Facility != null)
        {
            UserInteract.AskVariants("", new string[] {"Информация", "Снять с назначения", "Переназначить"}, new int[] {0, 1, 2}, RightMouseWork);

        }
        else
        {
            UserInteract.AskVariants("", new string[] { "Назначить" }, new int[] { 2 }, RightMouseWork);
        }
    }
    public void RightMouseWork(int index)
    {
        switch (index)
        {
            case 0:
                FindObjectOfType<WindowCreator>().CreateWindow<FacilityWindow>().SetInfo(Bear._Facility);
                break;
            case 1:
                Bear._Facility.AssignBear(Bear, true);
                break;
            case 2:
                UserInteract.AskFacility("Назначить медведя", SetWork);
                break;
        }
    }
    public void SetWork(Facility facility)
    {
        if(Bear._Facility != null)
        {
            Bear._Facility.AssignBear(Bear, true);
        }

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
            UserInteract.AskVariants("", new string[] { "Назначить" }, new int[] { 2 }, RightMouseHome);
        }
    }
    public void RightMouseHome(int index)
    {
        switch (index)
        {
            case 0:
                FindObjectOfType<WindowCreator>().CreateWindow<FacilityWindow>().SetInfo(Bear._Home);
                break;
            case 1:
                Bear._Home.AssignBear(Bear, true);
                break;
            case 2:
                UserInteract.AskFacility("Назначить медведя", SetHome);
                break;
        }
    }
    public void SetHome(Facility facility)
    {
        if (Bear._Home != null)
        {
            Bear._Home.AssignBear(Bear, true);
        }

        facility.AssignBear(Bear, false);
    }
}
