using System.Collections.Generic;
using UnityEngine;
using static CityStatistics;

public class CityStatistics : MonoBehaviour
{
    private int ControlHour = 0;
   [SerializeField] private Statistic[] Statistics = new Statistic[0];

    private float Electricity = 0   ;
    private float Honey = 0;
    private float EnergyHoney = 0;
    private float Wood = 0;
    private float Metal = 0;
    private float Berezenium = 0;
    private float Robots = 0;
    private float Snowrunners = 0;
    private float Astressin = 0;
    private float Ratonik = 0;
    private float Steamulator = 0;
    private float Antisleep = 0;

    public Statistic[] _Statistic => Statistics;
    public int _ControlHour => ControlHour;
    public string _SaveInfo
    {
        get
        {
            string info = $"Controls({Electricity};{Honey};{EnergyHoney};{Wood};{Metal};{Berezenium};{Robots};{Snowrunners};{Astressin};{Ratonik};{Steamulator};{Antisleep})Hour({ControlHour})Count({Statistics.Length})";

            for(int i = 0; i < Statistics.Length; i++)
            {
                info += $"S{i}(C({Statistics[i].Cause})H({Statistics[i].Hour})V({(int)(Statistics[i].Value * 100) / 100f})R({Statistics[i].Resource.GetHashCode()}))";
            }

            return info;
        }
        set
        {
            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            ControlHour = StaticTools.StringToInt(parameters["Hour"]);

            string[] controls = parameters["Controls"].Split(";");
            Electricity = StaticTools.StringToFloat(controls[0]);
            Honey = StaticTools.StringToFloat(controls[1]);
            EnergyHoney = StaticTools.StringToFloat(controls[2]);
            Wood = StaticTools.StringToFloat(controls[3]);
            Metal = StaticTools.StringToFloat(controls[4]);
            Berezenium = StaticTools.StringToFloat(controls[5]);
            Robots = StaticTools.StringToFloat(controls[6]);
            Snowrunners = StaticTools.StringToFloat(controls[7]);
            Astressin = StaticTools.StringToFloat(controls[8]);
            Ratonik = StaticTools.StringToFloat(controls[9]);
            Steamulator = StaticTools.StringToFloat(controls[10]);
            Antisleep = StaticTools.StringToFloat(controls[11]);

            int count = StaticTools.StringToInt(parameters["Count"]);
            Statistics = new Statistic[count];
            for(int i = 0; i < count; i++)
            {
                Dictionary<string, string> statistics = StaticTools.GetParameters(parameters[$"S{i}"]);
                Statistics[i] = new Statistic(statistics["C"], StaticTools.StringToInt(statistics["H"]), StaticTools.StringToFloat(statistics["V"]), (CityStorage.ResourceType)StaticTools.StringToInt(statistics["R"]));
            }
        }
    }

    public float GetControl(CityStorage.ResourceType resource)
    {
        switch (resource)
        {
            case CityStorage.ResourceType.Electricity:
                return Electricity;
            case CityStorage.ResourceType.Honey:
                return Honey;
            case CityStorage.ResourceType.EnergyHoney:
                return EnergyHoney;
            case CityStorage.ResourceType.Wood:
                return Wood;
            case CityStorage.ResourceType.Metal:
                return Metal;
            case CityStorage.ResourceType.Berezenium:
                return Berezenium;
            case CityStorage.ResourceType.Robots:
                return Robots;
            case CityStorage.ResourceType.Snowrunners:
                return Snowrunners;
            case CityStorage.ResourceType.Astressin:
                return Astressin;
            case CityStorage.ResourceType.Ratonik:
                return Ratonik;
            case CityStorage.ResourceType.Steamulator:
                return Steamulator;
            case CityStorage.ResourceType.Antisleep:
                return Antisleep;
        }

        return 0;
    }

    public Statistic[] GetStatistic(CityStorage.ResourceType resource)
    {
        int count = 0;
        foreach(Statistic statistic1 in Statistics)
        {
            if(statistic1.Resource == resource)
            {
                count++;
            }
        }

        int index = 0;
        Statistic[] statistic = new Statistic[count];
        for(int i = 0; i < Statistics.Length; i++)
        {
            if (Statistics[i].Resource == resource)
            {
                statistic[index] = Statistics[i];
                index++;
            }
        }

        return statistic;
    }

    public void SetControls()
    {
        ControlHour = (int)(City._Time._WorldTime / 60);

        int index = 0;
        while (index < Statistics.Length)
        {
            if (Statistics[index].Hour < ControlHour - 25)
            {
                Statistics = StaticTools.ReduceMassive(Statistics, index);
                index--;
            }
            index++;
        }

        Electricity = City._Energosystem._StoredEnergy;
        Honey = City._Foodstream._StoredFood;
        EnergyHoney = City._Storage._EnergyHoney;
        Wood = City._Storage._Wood;
        Metal = City._Storage._Metal;
        Berezenium = City._Storage._Berezenium;
        Robots = City._Storage._Robots;
        Snowrunners = City._Storage._Snowrunners;
        Antisleep = City._Storage._Antisleep;
    }

    public void AddStatistic(Statistic statistic)
    {
        Statistics = StaticTools.ExpandMassive(Statistics, statistic);
    }

    [System.Serializable]
    public class Statistic
    {
        public string Cause;
        public int Hour;
        public float Value;
        public CityStorage.ResourceType Resource;

        public Statistic(string cause, int hour, float value, CityStorage.ResourceType resource)
        {
            Cause = cause;
            Hour = hour;
            Value = value;
            Resource = resource;
        }
    }
}