using System.Xml.Linq;
using UnityEngine;

public class FoodProducer : Facility
{
    [SerializeField] private float BaseProduce;
    [SerializeField] private int HeatBonus;
    [SerializeField] protected float HeatCoefficientValue;

    public override int _ColdEndurance => base._ColdEndurance + Mathf.RoundToInt(HeatBonus * City._Energosystem._Effectivity) + (City._Research.GetResearchLevel(CityResearch.ResearchType.Food) >= 1 ? 3 : 0);
    public float _BaseProduce => BaseProduce;
    public override float _Effectivity
    {
        get
        {
            if (Bears.Length < 1)
            {
                return 0;
            }

            float work = 0;
            foreach (Bear bear in Bears)
            {
                work += bear._Work;
            }

            return work * HeatCoefficient() * Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2) ; 
        }
    }

    protected virtual float HeatCoefficient()
    {
        int heatLevel = _ColdEndurance - City._Weather._Cold;
        if(heatLevel >= 0)
        {
            return 1 + heatLevel * HeatCoefficientValue;
        }
        else if(heatLevel > -2)
        {
            return 0.5f;
        }
        else
        {
            return 0;
        }
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {ConstructInfo.Name}" +
            $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
            $"\nПроизводство мёда: {Mathf.RoundToInt(_BaseProduce * _Effectivity)} ед/ч" +
            $"{(_ColdEndurance - City._Weather._Cold < 0 ? "\n<color=red>Холодно !</color>" : "")}" +
            $"\n/////////";
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[2];

        float work = 0;
        foreach (Bear bear in Bears)
        {
            work += bear._Work;
        }

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2), true);

        int heatLevel = _ColdEndurance - City._Weather._Cold;
        if (heatLevel >= 0)
        {
            factors[1] = new CityFactors.Factor($"Тепло", 1 + heatLevel * HeatCoefficientValue, true);
        }
        else if(heatLevel > -2)
        {
            factors[1] = new CityFactors.Factor($"Холодно", 0.5f, true);
        }
        else
        {
            factors[1] = new CityFactors.Factor($"Заморожено", 0, true);
        }

        if (Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0, true));
        }

        return factors;
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

        City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), BaseProduce * _Effectivity , CityStorage.ResourceType.Honey));
        City._Foodstream._StoredFood += BaseProduce * _Effectivity;

        foreach (Bear bear in Bears)
        {
            bear._Tired += CityTime._DaySection * Tiring * bear._TiredCoefficient;
        }
    }
}
