using UnityEngine;

public class EnergyProcuder : Facility
{
    [SerializeField] private CityStorage.ResourceType ResourceType;
    [SerializeField] private float BaseConsume;
    [SerializeField] private float BaseProduce;

    public CityStorage.ResourceType _Resource => ResourceType;
    public float _BaseConsume => BaseConsume * (City._Research.GetResearchLevel(CityResearch.ResearchType.Electricity) >= 2 ? 0.85f : 1);
    public float _BaseProduce => BaseProduce ;
    public  float _Produce => BaseProduce * _Effectivity;
    public override float _Effectivity
    {
        get
        {
            float summSkill = 0;
            foreach (Bear bear in AssignedBears)
            {
                summSkill += bear._Skill;
            }

            return Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f) * (summSkill < _RequiredSkill ? 0.5f : 1) * (City._Buyiments._EffectivityBoost ? 1.25f : 1) * (City._Research.GetResearchLevel(CityResearch.ResearchType.Electricity) >= 2 ? 1.25f : 1) * (Enabled ? 1 : 0);
        }
    }

    public override float GetPotencialEffectivity()
    {
        float summSkill = 0;
        foreach (Bear bear in AssignedBears)
        {
            summSkill += bear.GetSkill(City._Foodstream._Potencial[2]);
        }

        return Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f) * (summSkill < _RequiredSkill ? 0.5f : 1) * (City._Buyiments._EffectivityBoost ? 1.25f : 1) * (City._Research.GetResearchLevel(CityResearch.ResearchType.Electricity) >= 2 ? 1.25f : 1) * (Enabled ? 1 : 0);
    }
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[2];

        float summSkill = 0;
        foreach (Bear bear in AssignedBears)
        {
            summSkill += bear._Skill;
        }

        factors[0] = new CityFactors.Factor($"Опыт медведей", Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f), -1, true);

        if (summSkill < _RequiredSkill)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Неопытность сотрудников", 0.5f, -1, true));
        }

        if (City._Buyiments._EffectivityBoost)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Промышленные нановнедрения", 1.25f, -1, true));
        }

        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Electricity) >= 2)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Разработки в электросистеме", 1.25f, -1, true));
        }

        if (!Enabled)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Работа приостановлена", 0, -1, true));
        }

        return factors;
    }
}
