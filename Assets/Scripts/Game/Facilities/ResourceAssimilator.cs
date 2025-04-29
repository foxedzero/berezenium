
using System.Collections.Generic;
using UnityEngine;

public class ResourceAssimilator : Assimilator
{
    [SerializeField] private ResourceField ResourceField;

    public override int _Order => 2;

    public override string _SaveInfo
    {
        get
        {
            return base._SaveInfo + $"Field({StaticTools.IndexOf(City._CityGeology._Fields, ResourceField)})";
        }
        set
        {
            base._SaveInfo = value;
        }
    }

    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        SetField(City._CityGeology._Fields[StaticTools.StringToInt(parameters["Field"])]);
    }

    public override void HourPassed()
    {
        if (Bears.Length <= 0)
        {
            return;
        }

        switch (HeaterOn)
        {
            case 1:
                if (City._Storage._EnergyHoney >= 1)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Обогреватель {ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), -1, CityStorage.ResourceType.EnergyHoney));
                    City._Storage._EnergyHoney -= 1;
                    Heated += 3;
                }
                break;
            case 2:
                if (City._Storage._Wood >= 5)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Обогреватель {ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), -5, CityStorage.ResourceType.Wood));
                    City._Storage._Wood -= 5;
                    Heated += 2;
                }
                break;
        }

        float value = 0;
        foreach (Bear bear in Bears)
        {
            bear._Tired += CityTime._DaySection * Tiring * bear._TiredCoefficient;
        }

        value = Mathf.Max(0, GetNominal() * _Effectivity);
        City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), value, ConstructInfo.MiningResource));
        City._Storage.AddResource(ConstructInfo.MiningResource, value);

        SmtChanged();
    }

    public void SetField(ResourceField field)
    {
        ResourceField = field;
        transform.localRotation = ResourceField.transform.localRotation;
    }
}
