using UnityEngine;

public class Laboratory : Facility
{
    [SerializeField] private CityResearch.ResearchType TargetResearch;
    [SerializeField] private float BasePoints;
    [SerializeField] private bool Small;

    public override event SimpleVoid OnSmthChange = null;

    public override bool _CanBeDisabled => false;

    public CityResearch.ResearchType _Target
    {
        get
        {
            return TargetResearch;
        }
        set
        {
            TargetResearch = value;

            switch (TargetResearch)
            {
                case CityResearch.ResearchType.Electricity:
                    RequiredKasta = Bear.Kasta.Программист;
                    break;
                case CityResearch.ResearchType.Cold:
                    RequiredKasta = Bear.Kasta.Конструктор;
                    break;
                case CityResearch.ResearchType.Medicine:
                    RequiredKasta = Bear.Kasta.Биоинженер;
                    break;
                case CityResearch.ResearchType.Travels:
                    RequiredKasta = Bear.Kasta.Первопроходец;
                    break;
                case CityResearch.ResearchType.Household:
                    RequiredKasta = Bear.Kasta.Конструктор;
                    break;
                case CityResearch.ResearchType.Food:
                    RequiredKasta = Bear.Kasta.Биоинженер;
                    break;
                case CityResearch.ResearchType.Production:
                    RequiredKasta = Bear.Kasta.Конструктор;
                    break;
                case CityResearch.ResearchType.Mining:
                    RequiredKasta = Bear.Kasta.Программист;
                    break;
            }
            
            City._Research.LabolatoryRetarget(this);

                OnSmthChange?.Invoke();
        }
    }
    public float _BaseResearchPoints => BasePoints;
    public float _ResearchPoints
    {
        get
        {
            return BasePoints * _Effectivity;
        }
    }
    public override float _Effectivity
    {
        get
        {
            float summSkill = 0;
            foreach (Bear bear in AssignedBears)
            {
                summSkill += bear._Skill;
            }

            return Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f) * Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2) * (City._Buyiments._EffectivityBoost ? 1.25f : 1);
        }
    }
    public override int _Heater 
    { 
        get => base._Heater;
        set
        {
            base._Heater = value;

            OnSmthChange?.Invoke();
        }
    }
    public override int _Heated
    { get => base._Heated; set
        {
            base._Heated = value;

            OnSmthChange?.Invoke();
        }
    }

    public override string _SaveInfo 
    { 
        get => base._SaveInfo + $"Target({TargetResearch.GetHashCode()})";
        set
        {
            base._SaveInfo = value;
            
            _Target = (CityResearch.ResearchType)StaticTools.StringToInt(StaticTools.GetParameter(value, "Target"));
        }
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
        
        if (City._Buyiments._EffectivityBoost)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Промышленные нановнедрения", 1.25f, -1, true));
        }

        return factors;
    }
    public override float GetPotencialEffectivity()
    {
        float summSkill = 0;
        foreach (Bear bear in AssignedBears)
        {
            summSkill += bear.GetSkill(City._Foodstream._Potencial[2]);
        }

        return Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f) * Mathf.Clamp(Mathf.Pow(City._Energosystem._Potencial[2], 2), MinimalEnergyCoeffiente, 2) * (City._Buyiments._EffectivityBoost ? 1.25f : 1);
    }

    public override bool AssignBear(Bear bear, bool remove)
    {
        int index = StaticTools.IndexOf(AssignedBears, bear);
        if (remove)
        {
            if (index > -1)
            {
                AssignedBears = StaticTools.ReduceMassive(AssignedBears, index);

                bear._Facility = null;

                if (OnSmthChange != null)
                {
                    OnSmthChange.Invoke();
                }

                return true;
            }
        }
        else
        {
            if (index < 0 && AssignedBears.Length < _MaxBearCount)
            {
                AssignedBears = StaticTools.ExpandMassive(AssignedBears, bear);

                if (bear._Facility != null)
                {
                    bear._Facility.AssignBear(bear, true);
                }

                bear._Facility = this;

                if (OnSmthChange != null)
                {
                    OnSmthChange.Invoke();
                }

                return true;
            }
        }

        return false;
    }

    public void SetRequiredSkill(float value)
    {
        return;

        if (Small)
        {
            RequiredSkill = value / 2;
        }
        else
        {
            RequiredSkill = value;
        }

            OnSmthChange?.Invoke();
    }

    public override void RightMouseActions(int index)
    {
        base.RightMouseActions(index);
        OnSmthChange?.Invoke();
    }

    public override void SetHeater(int index)
    {
        base.SetHeater(index);
        OnSmthChange?.Invoke();
    }
}
