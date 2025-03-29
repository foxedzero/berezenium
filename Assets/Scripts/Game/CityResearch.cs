
using System.Collections.Generic;
using UnityEngine;

public class CityResearch : MonoBehaviour
{
    public enum ResearchType {Electricity, Cold, Medicine, Travels, Household, Food, Production, Mining}

    [SerializeField] private float Electricity;
    [SerializeField] private float Cold;
    [SerializeField] private float Medicine;
    [SerializeField] private float Travels;
    [SerializeField] private float Household;
    [SerializeField] private float Food;
    [SerializeField] private float Production;
    [SerializeField] private float Mining;

    private Laboratory[] Laboratories = new Laboratory[0];

    public event SimpleVoid OnReseatchUpdate = null;

    public string _SaveInfo
    {
        get
        {
            return $"Electricity({Electricity})Cold({Cold})Medicine({Medicine})Travels({Travels})Household({Household})Food({Food})Production({Production})Mining({Mining})";
        }
        set
        {
            if (value == null || value.Length <=0)
            {
                return;
            }

            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            Electricity = StaticTools.StringToFloat(parameters["Electricity"]);
            Cold = StaticTools.StringToFloat(parameters["Cold"]);
            Medicine = StaticTools.StringToFloat(parameters["Medicine"]);
            Travels = StaticTools.StringToFloat(parameters["Travels"]);
            Household = StaticTools.StringToFloat(parameters["Household"]);
            Food = StaticTools.StringToFloat(parameters["Food"]);
            Production = StaticTools.StringToFloat(parameters["Production"]);
            Mining = StaticTools.StringToFloat(parameters["Mining"]);
        }
    }

    private void Start()
    {
        City._DataBase.OnFacilityChanges += UpdateLaboratories;
    }

    public void UpdateLaboratories()
    {
        Laboratories = new Laboratory[0];

        foreach(Facility facility in City._DataBase._Facilities)
        {
            Laboratory laboratory = facility as Laboratory;
            if(laboratory != null)
            {
                Laboratories = StaticTools.ExpandMassive(Laboratories, laboratory);
            }
        }
    }

    public Bear.Kasta ResearchKasta(ResearchType type)
    {
        switch (type)
        {
            case CityResearch.ResearchType.Electricity:
                return Bear.Kasta.Конструктор;
            case CityResearch.ResearchType.Cold:
                return Bear.Kasta.Конструктор;
            case CityResearch.ResearchType.Medicine:
                return Bear.Kasta.Биоинженер;
            case CityResearch.ResearchType.Travels:
                return Bear.Kasta.Первопроходец;
            case CityResearch.ResearchType.Household:
                return Bear.Kasta.Конструктор;
            case CityResearch.ResearchType.Food:
                return Bear.Kasta.Биоинженер;
            case CityResearch.ResearchType.Production:
                return Bear.Kasta.Конструктор;
            case CityResearch.ResearchType.Mining:
                return Bear.Kasta.Конструктор;
        }
        return Bear.Kasta.Неопределено;
    }

    public float GetResearchProgress(ResearchType type, int level)
    {
        float points = 0;
        switch (type)
        {
            case ResearchType.Electricity:
                points = Electricity;
                break;
            case ResearchType.Cold:
                points = Cold;
                break;
            case ResearchType.Medicine:
                points = Medicine;
                break;
            case ResearchType.Travels:
                points = Travels;
                break;
            case ResearchType.Household:
                points = Household;
                break;
            case ResearchType.Food:
                points = Food;
                break;
            case ResearchType.Production:
                points = Production;
                break;
            case ResearchType.Mining:
                points = Mining;
                break;
        }

        switch (level)
        {
            case 0:
                return Mathf.Clamp01(points / 1000f);
            case 1:
                points -= 1000;
                if (points > 1000)
                {
                    return 1;
                }
                return Mathf.Clamp01(points / 1000f);
            case 2:
                points -= 2000;
                if (points > 2000)
                {
                    return 1;
                }
                return Mathf.Clamp01(points / 2000f);
        }

        return 0;
    }

    public int GetResearchLevel(ResearchType type)
    {
        float score = GetResearch(type);
        if (score >= 4000)
        {
            return 3;
        }
        else if (score >= 2000)
        {
            return 2;
        }
        else if (score >= 1000)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public float GetResearch(ResearchType type)
    {
        switch (type)
        {
            case ResearchType.Electricity:
                return Electricity;
            case ResearchType.Cold:
                return Cold;
            case ResearchType.Medicine:
                return Medicine;
            case ResearchType.Travels:
                return Travels;
            case ResearchType.Household:
                return Household;
            case ResearchType.Food:
                return Food;
            case ResearchType.Production:
                return Production;
            case ResearchType.Mining:
                return Mining;
        }

        return 0;
    }

    public void SetResearch(ResearchType type, float value)
    {
        switch (type)
        {
            case ResearchType.Electricity:
                Electricity = value;
                break;
            case ResearchType.Cold:
                Cold = value;
                break;
            case ResearchType.Medicine:
                Medicine = value;
                break;
            case ResearchType.Travels:
                Travels = value;
                break;
            case ResearchType.Household:
                Household = value;
                break;
            case ResearchType.Food:
                Food = value;
                break;
            case ResearchType.Production:
                Production = value;
                break;
            case ResearchType.Mining:
                Mining = value;
                break;
        }

        OnReseatchUpdate?.Invoke();
    }

    public void Research(ResearchType type, float value)
    {
        int level = GetResearchLevel(type);

        switch (type)
        {
            case ResearchType.Electricity:
                Electricity += value;
                break; 
            case ResearchType.Cold:
                Cold += value;
                break;
            case ResearchType.Medicine:
                Medicine += value;
                break;
            case ResearchType.Travels:
                Travels += value;
                break;
            case ResearchType.Household:
                Household += value;
                break;
            case ResearchType.Food:
                Food += value;
                break;
            case ResearchType.Production:
                Production += value;
                break;
            case ResearchType.Mining:
                Mining += value;
                break;
        }

        int newLv = GetResearchLevel(type);
        if (level != newLv)
        {
            CityMessenger.CityMessage message = new CityMessenger.CityMessage("", "");
            
            switch (type)
            {
                case ResearchType.Electricity:
                    message.Label = $"Электроэнергия {City._Research.GetResearchLevel(ResearchType.Electricity)} уровня";
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Исследована электроэнергия 1 уровня. Теперь вы можете строить аккумуляторы, которые позволят вам хранить избыточное электричество.";
                            break;
                        case 2:
                            message.Info = $"Исследована электроэнергия 2 уровня. Эффективность электростанции увеличена на 10%. Общая эффективность энергосистемы теперь 110%.";
                            break;
                        case 3:
                            message.Info = $"Исследована электроэнергия 3 уровня. Разработана технология получения энергии на основе магнитно-гравитационных свойств березениума. Теперь вы можете построить березениумную электростанцию.";
                            break;
                    }
                    break;
                case ResearchType.Cold:
                    message.Label = $"Отопление {City._Research.GetResearchLevel(ResearchType.Cold)} уровня";
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Исследовано отопление 1 уровня. С этого момента в зданиях доступны обогреватели, их можно включить в окне управления зданием.";
                            break;
                        case 2:
                            message.Info = $"Исследовано отопление 2 уровня. Электростанции будут отоплять ближайшие здания.";
                            break;
                        case 3:
                            message.Info = $"Исследовано отопление 3 уровня. С этого момента вы можете строить уличные отопители для обогрева близлежащих зданий.";
                            break;
                    }
                    break;
                case ResearchType.Medicine:
                    message.Label = $"Медицина {City._Research.GetResearchLevel(ResearchType.Medicine)} уровня";
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Исследована медицина 1 уровня. Теперь вы можете построить здание фармацевтики, где можно будет сделать Астрессин и Дажьтоник.";
                            break;
                        case 2:
                            message.Info = $"Исследована медицина 2 уровня. Эффективность медведей, работающих в медпункте, увеличилась на 50%.";
                            break;
                        case 3:
                            message.Info = $"Исследована медицина 3 уровня. Вам стало доступно изготовление Антиспячкина и стимуляторов.";
                            break;
                    }
                    break;
                case ResearchType.Travels:
                    message.Label = $"Путешествия {City._Research.GetResearchLevel(ResearchType.Travels)} уровня";
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Исследованы путешествия 1 уровня. Медведи в составе отряда разведки теперь защищены от холода на 4 единицы лучше.";
                            break;
                        case 2:
                            message.Info = $"Исследованы путешествия 2 уровня. Вам стало доступно производство снегоходов на заводе робототехники.";
                            break;
                        case 3:
                            message.Info = $"Исследованы путешествия 3 уровня. С этого момента вы можете построить космолет, чтобы покинуть планету и вернуться на родину.";
                            break;
                    }
                    break;
                case ResearchType.Household:
                    message.Label = $"Жилища {City._Research.GetResearchLevel(ResearchType.Household)} уровня";
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Исследованы жилища 1 уровня. Теперь вы можете построить многоэтажный дом, который лучше защищен от холода, чем барак, но требует больших затрат на строительство.";
                            break;
                        case 2:
                            message.Info = $"Исследованы жилища 2 уровня. Защита домов от холода увеличена на 2 единицы.";
                            break;
                        case 3:
                            message.Info = $"Исследованы жилища 3 уровня. С этого момента вы можете построить особняк.";
                            break;
                    }
                    break;
                case ResearchType.Food:
                    message.Label = $"Пища {City._Research.GetResearchLevel(ResearchType.Food)} уровня";
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Исследована пища 1 уровня. Защита пасек от холода увеличена на 3 единицы.";
                            break;
                        case 2:
                            message.Info = $"Исследована пища 2 уровня. Теперь вы можете построить экопасеку.";
                            break;
                        case 3:
                            message.Info = $"Исследована пища 3 уровня. С этого момента вам стало доступно строительство киберпасеки. На заводах робототехники можно собирать киберпчёл.";
                            break;
                    }
                    break;
                case ResearchType.Production:
                    message.Label = $"Производство {City._Research.GetResearchLevel(ResearchType.Production)} уровня";
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Исследованы производства 1 уровня. Теперь вы можете построить химический завод энергомёда.";
                            break;
                        case 2:
                            message.Info = $"Исследованы производства 2 уровня. Производства стали эффективнее на 25%.";
                            break;
                        case 3:
                            message.Info = $"Исследованы производства 3 уровня. Вам стала доступна постройка завода двойной обработки березениума.";
                            break;
                    }
                    break;
                case ResearchType.Mining:
                    message.Label = $"Добыча {City._Research.GetResearchLevel(ResearchType.Mining)} уровня";
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Исследована добыча 1 уровня. Защита от холода точек добычи повышена на 3 единицы, а эффективность их работы увеличена на 10%.";
                            break;
                        case 2:
                            message.Info = $"Исследована добыча 2 уровня. Теперь вы можете построить карьер березениума.";
                            break;
                        case 3:
                            message.Info = $"Исследована добыча 3 уровня. С этого момента эффективность добычи увеличена еще на 25%.";
                            break;
                    }
                    break;
            }

            City._CityMessenger.AddMessage(message);
        }

        if (OnReseatchUpdate != null)
        {
            OnReseatchUpdate.Invoke();
        }
    }
}
