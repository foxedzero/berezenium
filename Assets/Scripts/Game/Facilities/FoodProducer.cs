using UnityEngine;

public class FoodProducer : Facility
{
    [SerializeField] private float BaseProduce;

    public override bool _CanBeDisabled => false;
    public override int _ColdEndurance => base._ColdEndurance + (City._Research.GetResearchLevel(CityResearch.ResearchType.Food) >= 1 ? 3 : 0);
    public float _Produce => (BaseProduce + (City._Research.GetResearchLevel(CityResearch.ResearchType.Food) >= 2 ? 10 : 0)) * _Effectivity;
    public float _BaseProduce => BaseProduce;
    public override float _Effectivity
    {
        get
        {
            float summSkill = 0;
            foreach (Bear bear in AssignedBears)
            {
                summSkill += bear._Skill;
            }

            return Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f) * Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2) * (summSkill < _RequiredSkill ? 0.5f : 1) * (City._Buyiments._EffectivityBoost ? 1.25f : 1); 
        }
    }

    public override float GetPotencialEffectivity()
    {
        float summSkill = 0;
        foreach (Bear bear in AssignedBears)
        {
            summSkill += bear._Skill;
        }

        return Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f) * Mathf.Clamp(Mathf.Pow(City._Energosystem._Potencial[2], 2), MinimalEnergyCoeffiente, 2) * (summSkill < _RequiredSkill ? 0.5f : 1) * (City._Buyiments._EffectivityBoost ? 1.25f : 1);
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
        factors[1] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2), -1, true);

        if (summSkill < _RequiredSkill)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Неопытность сотрудников", 0.5f, -1, true));
        }

        if (City._Buyiments._EffectivityBoost)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Промышленные нановнедрения", 1.25f, -1, true));
        }

        return factors;
    }
}
