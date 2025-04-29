using System.Collections.Generic;
using UnityEngine;

public class CityStorage : MonoBehaviour
{
    public enum ResourceType { EnergyHoney , Berezenium , Metal , Wood , Robots, Snowrunners, Honey, Electricity, Astressin, Ratonik, Steamulator, Antisleep, CyberBee, None = -1 }

    [Header("Ресурсы")]
    [SerializeField] private float EnergyHoney;
    [SerializeField] private float Berezenium;
    [SerializeField] private float Metal;
    [SerializeField] private float Wood;
    [SerializeField] private int Robots;
    [SerializeField] private int CyberBee;
    [SerializeField] private int Snowrunners;
    [SerializeField] private int Antisleep;

    public event SimpleVoid OnChanges = null;
    public event SimpleVoid OnDrugChanges = null;

    public float _EnergyHoney
    {
        get
        {
            return EnergyHoney;
        }
        set
        {
            EnergyHoney = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public float _Berezenium
    {
        get
        {
            return Berezenium;
        }
        set
        {
            Berezenium = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public float _Metal
    {
        get
        {
            return Metal;
        }
        set
        {
            Metal = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public float _Wood
    {
        get
        {
            return Wood;
        }
        set
        {
            Wood = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public int _Robots
    {
        get
        {
            return Robots;
        }
        set
        {
            Robots = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public int _CyberBee
    {
        get
        {
            return CyberBee;
        }
        set
        {
            CyberBee = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public int _Snowrunners
    {
        get
        {
            return Snowrunners;
        }
        set
        {
            Snowrunners = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }

    public int _Antisleep
    {
        get
        {
            return Antisleep;
        }
        set
        {
            Antisleep = value;
            OnDrugChanges?.Invoke();
        }
    }

    public string _SaveInfo
    {
        get
        {
            return $"EHoney({EnergyHoney})Berezenium({Berezenium})Metal({Metal})Wood({Wood})Robots({Robots})Bee({CyberBee})Snowrunners({Snowrunners})Antisleep({Antisleep})";
        }
        set
        {
            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            EnergyHoney = StaticTools.StringToFloat(parameters["EHoney"]);
            Berezenium = StaticTools.StringToFloat(parameters["Berezenium"]);
            Metal = StaticTools.StringToFloat(parameters["Metal"]);
            Wood = StaticTools.StringToFloat(parameters["Wood"]);
            Robots = StaticTools.StringToInt(parameters["Robots"]);
            CyberBee = StaticTools.StringToInt(parameters["Bee"]);
            Snowrunners = StaticTools.StringToInt(parameters["Snowrunners"]);

            Antisleep = StaticTools.StringToInt(parameters["Antisleep"]);
        }
    }

    public void AddContent(CitySally.TileContent content)
    {
        int hour = (int)(City._Time._WorldTime / 60);
        
        if(content.EnergyHoney > 0)
        {
            City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Ресурсы с вылазок", hour, content.EnergyHoney, ResourceType.EnergyHoney));
            EnergyHoney += content.EnergyHoney;
        }
        if (content.Berezenium > 0)
        {
            City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Ресурсы с вылазок", hour, content.Berezenium, ResourceType.Berezenium));
            Berezenium += content.Berezenium;
        }
        if (content.Metal > 0)
        {
            City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Ресурсы с вылазок", hour, content.Metal, ResourceType.Metal));
            Metal += content.Metal;
        }
        if (content.Wood > 0)
        {
            City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Ресурсы с вылазок", hour, content.Wood, ResourceType.Wood));
            Wood += content.Wood;
        }
        if (content.Robots > 0)
        {
            City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Ресурсы с вылазок", hour, content.Robots, ResourceType.Robots));
            Robots += content.Robots;
        }

        OnChanges?.Invoke();
    }

    public float GetResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.EnergyHoney:
                return EnergyHoney;
            case ResourceType.Berezenium:
                return Berezenium;
            case ResourceType.Metal:
                return Metal;
            case ResourceType.Wood:
                return Wood;
            case ResourceType.Robots:
                return Robots;
            case ResourceType.CyberBee:
                return CyberBee;
            case ResourceType.Snowrunners:
                return Snowrunners;
            case ResourceType.Honey:
                return City._Foodstream._StoredFood;
        }

        return 0;
    }

    public void AddResource(ResourceType type, float amount)
    {
        switch (type)
        {
            case ResourceType.EnergyHoney:
               EnergyHoney += amount;
                break;
            case ResourceType.Berezenium:
                Berezenium += amount;
                break;
            case ResourceType.Metal:
                Metal += amount;
                break;
            case ResourceType.Wood:
                Wood += amount;
                break;
            case ResourceType.Robots:
                Robots += (int)amount;
                break;
            case ResourceType.CyberBee:
                CyberBee += (int)amount;
                break;
            case ResourceType.Snowrunners:
                Snowrunners += (int)amount;
                break;
            case ResourceType.Honey:
                City._Foodstream._StoredFood += amount;
                break;
        }
    }

    public static string ResourceName(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.EnergyHoney:
                return "Энергомёд";
            case ResourceType.Berezenium:
                return "Березениум";
            case ResourceType.Metal:
                return "Металл";
            case ResourceType.Wood:
                return "Древесина";
            case ResourceType.Robots:
                return "Роботы";
            case ResourceType.CyberBee:
                return "Киберпчёлы";
            case ResourceType.Snowrunners:
                return "Снегоходы";
            case ResourceType.Honey:
                return "Мёд";
            case ResourceType.Astressin:
                return "Астрессин";
            case ResourceType.Ratonik:
                return "Дажьтоник";
            case ResourceType.Steamulator:
                return "Стимул!";
            case ResourceType.Antisleep:
                return "Антиспячкин";
        }

        return "";
    }
}
