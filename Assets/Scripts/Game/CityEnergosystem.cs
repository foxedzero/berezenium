using System.Collections.Generic;
using UnityEngine;
using static Constructor;

public class CityEnergosystem : MonoBehaviour
{
    [SerializeField] private CityTime CityTime;
    [SerializeField] private CityDataBase CityData;

    [SerializeField] private float StoredEnergy;
    [SerializeField] private float CurrentEnergy;
    [SerializeField] private float EnergyCapacity;
    [SerializeField] private float EffectivityCoefficient;

    public string _SaveInfo
    {
        get
        {
            return $"Stored({StoredEnergy})Energosystem({EffectivityCoefficient})";
        }
        set
        {
            if(value == null || value.Length <= 0)
            {
                return;
            }

            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            StoredEnergy = StaticTools.StringToFloat(parameters["Stored"]);
            EffectivityCoefficient = StaticTools.StringToFloat(parameters["Energosystem"]);
        }
    }
    public float _Effectivity => EffectivityCoefficient;
    public float _CurrentEnergy
    {
        get
        {
            return CurrentEnergy;
        }
        set
        {
            CurrentEnergy = value;
        }
    }
    public float _StoredEnergy
    {
        get
        {
            return StoredEnergy;
        }
        set
        {
           StoredEnergy = value;
        }
    }
    public float _EnergyCapacity => EnergyCapacity;

    public float CurrentProduce()
    {
        float value = 0;
        foreach(Facility facility in City._DataBase._Facilities)
        {
            if(facility is EnergyProcuder)
            {
                value += (facility as EnergyProcuder)._Producing;
            }
        }

        return value;
    }
    public float CurrentConsume()
    {
        float consume = 0;

        foreach (Facility facility in CityData._Facilities)
        {
            consume += facility._EnergyConsume;
        }

        return consume;
    }

    private void Start()
    {
        City._DataBase.OnFacilityChanges += UpdateCapacity;
        UpdateCapacity();
    }

    public void UpdateCapacity()
    {
        EnergyCapacity = 0;
        foreach(Facility facility in City._DataBase._Facilities)
        {
            if(facility is Accumulator)
            {
                EnergyCapacity += (facility as Accumulator)._Capacity;
            }
            else if(facility is EnergyProcuder)
            {
                EnergyCapacity += (facility as EnergyProcuder)._EnergyCapacity;
            }
        }
    }
    public void HourPassed()
    {
        float current = CurrentEnergy;
        CurrentEnergy = 0;

        if (current + StoredEnergy <= 0)
        {
            EffectivityCoefficient = 0;
            return;
        }

        float consume = CurrentConsume();

        if (current + StoredEnergy < consume)
        {
            EffectivityCoefficient = (current + StoredEnergy) / consume;
            EffectivityCoefficient *= City._Research.GetResearchLevel(CityResearch.ResearchType.Electricity) >= 2 ? 1.1f : 1;

            City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Потребление электричества", (int)(City._Time._WorldTime / 60), -(current + StoredEnergy), CityStorage.ResourceType.Electricity));
            StoredEnergy = 0;
        }
        else
        {
            EffectivityCoefficient = City._Research.GetResearchLevel(CityResearch.ResearchType.Electricity) >= 2 ? 1.1f : 1;

            City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Потребление электричества", (int)(City._Time._WorldTime / 60), -consume, CityStorage.ResourceType.Electricity));
            StoredEnergy = Mathf.Clamp(StoredEnergy + current - consume, 0, _EnergyCapacity);
        }
    }
}
