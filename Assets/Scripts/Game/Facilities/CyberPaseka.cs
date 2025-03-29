using System;
using System.Collections.Generic;
using UnityEngine;

public class CyberPaseka : FoodProducer
{
    [SerializeField] private int MaxBees;
    [SerializeField] private int CyberBees;

    public int _MaxCyberBees => MaxBees;
    public int _CyberBees
    {
        get
        {
            return CyberBees;
        }
        set
        {
            CyberBees = value;
            SmtChanged();
        }
    }

    public float _RequiredWork => CyberBees == 0 ? 0.5f : CyberBees * 0.5f;
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

            return Mathf.Clamp(work/ _RequiredWork, 0, 1.25f) * HeatCoefficient() * CyberBees * Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2) ;
        }
    }

    public override string _SaveInfo { get => base._SaveInfo + $"Bees({CyberBees})"; set => base._SaveInfo = value; }
    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        CyberBees = StaticTools.StringToInt(parameters["Bees"]);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), CyberBees, CityStorage.ResourceType.CyberBee));
        City._Storage._CyberBee += CyberBees;
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {ConstructInfo.Name}" +
            $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nКиберпчёлы: {CyberBees}/{MaxBees}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
            $"\nПроизводство мёда: {Mathf.RoundToInt(_BaseProduce * _Effectivity)} ед/ч" +
            $"{(_ColdEndurance - City._Weather._Cold < 0 ? "\n<color=red>Холодно !</color>" : "")}" +
            $"\n/////////";
    }


    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[4];

        float work = 0;
        foreach (Bear bear in Bears)
        {
            work += bear._Work;
        }

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2), true);
        factors[1] = new CityFactors.Factor($"Работа/Требуемая", work / _RequiredWork, true);
        factors[2] = new CityFactors.Factor($"Киберпчёлы", CyberBees, true);

        int heatLevel = _ColdEndurance - City._Weather._Cold;
        if (heatLevel >= 0)
        {
            factors[3] = new CityFactors.Factor($"Тепло", 1 + heatLevel * HeatCoefficientValue, true);
        }
        else if (heatLevel > -2)
        {
            factors[3] = new CityFactors.Factor($"Холодно", 0.5f, true);
        }
        else
        {
            factors[3] = new CityFactors.Factor($"Заморожено", 0, true);
        }

        if (Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0, true));
        }

        return factors;
    }

}
