using UnityEngine;

public class Producer : Facility
{
    [SerializeField] protected int RequiredRobots;
    [SerializeField] protected int Robots;

    [SerializeField] protected CityStorage.ResourceType Resource;
    [SerializeField] protected float NominalCount;

    [SerializeField] protected CityStorage.ResourceType ConsumeResource;
    [SerializeField] protected float ConsumeCount;

    public override event SimpleVoid OnSmthChange = null;

    public CityStorage.ResourceType _ConsumeResource => ConsumeResource;
    public float _Consume => ConsumeCount * (City._Research.GetResearchLevel(CityResearch.ResearchType.Production) >= 2 ? 0.75f : 1);
    public CityStorage.ResourceType _ResourceType => Resource;
    public float _Nominal => NominalCount;
    public virtual int _RequiredRobots => RequiredRobots;
    public virtual int _Robots
    {
        get
        {
            return Robots;
        }
        set
        {
            Robots = value;

            if(OnSmthChange != null)
            {
                OnSmthChange.Invoke();
            }
        }
    }
    public override string _SaveInfo
    {
        get
        {
            return base._SaveInfo + $"Robots({Robots})";
        }
        set
        {
            base._SaveInfo = value;

            Robots = StaticTools.StringToInt(StaticTools.GetParameter(value, "Robots"));
        }
    }

    public override bool _Enabled
    {
        get => base._Enabled;
        set
        {
            base._Enabled = value;
            OnSmthChange?.Invoke();
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
    {
        get => base._Heated; set
        {
            base._Heated = value;

            OnSmthChange?.Invoke();
        }
    }

    public override float _Effectivity
    {
        get
        {
            return base._Effectivity * (Robots / (float)_RequiredRobots) * (Enabled ? 1 : 0);
        }
    }

    public override float GetPotencialEffectivity()
    {
        return base.GetPotencialEffectivity() * (Robots / (float)_RequiredRobots) * (Enabled ? 1 : 0);
    }
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = StaticTools.ExpandMassive(base.GetEffectivityFactors(), new CityFactors.Factor("Роботы", Robots / (float)_RequiredRobots, -1, true));

        if (!Enabled)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Работа приостановлена", 0, -1, true));
        }

        return factors;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        City._Time.DayChanged -= DayPassed;
    }

    protected override void Start()
    {
        base.Start();
        City._Time.DayChanged += DayPassed;
    }

    public virtual void DayPassed()
    {
        switch (ConsumeResource)
        {
            case CityStorage.ResourceType.EnergyHoney:
                if(City._Storage._EnergyHoney >= _Consume)
                {
                    City._Storage._EnergyHoney -= _Consume;
                }
                else
                {
                    return;
                }
                break;
            case CityStorage.ResourceType.Berezenium:
                if (City._Storage._Berezenium >= _Consume)
                {
                    City._Storage._Berezenium -= _Consume;
                }
                else
                {
                    return;
                }
                break;
            case CityStorage.ResourceType.Metal:
                if (City._Storage._Metal >= _Consume)
                {
                    City._Storage._Metal -= _Consume;
                }
                else
                {
                    return;
                }
                break;
            case CityStorage.ResourceType.Wood:
                if (City._Storage._Wood >= _Consume)
                {
                    City._Storage._Wood -= _Consume;
                }
                else
                {
                    return;
                }
                break;
            case CityStorage.ResourceType.Honey:
                if (City._Foodstream._StoredFood >= _Consume)
                {
                    City._Foodstream._StoredFood -= _Consume;
                }
                else
                {
                    return;
                }
                break;
        }

        switch (Resource)
        {
            case CityStorage.ResourceType.EnergyHoney:
                City._Storage._EnergyHoney += Mathf.Max(0, NominalCount * _Effectivity);
                break;
            case CityStorage.ResourceType.Berezenium:
                City._Storage._Berezenium += Mathf.Max(0, NominalCount * _Effectivity);
                break;
            case CityStorage.ResourceType.Metal:
                City._Storage._Metal += Mathf.Max(0, NominalCount * _Effectivity);
                break;
            case CityStorage.ResourceType.Wood:
                City._Storage._Wood += Mathf.Max(0, NominalCount * _Effectivity);
                break;
            case CityStorage.ResourceType.Robots:
                City._Storage._Robots += Mathf.Max(0, Mathf.FloorToInt(NominalCount * _Effectivity));
                break;
            case CityStorage.ResourceType.Honey:
                City._Foodstream._StoredFood += Mathf.Max(0, Mathf.FloorToInt(NominalCount * _Effectivity));
                break;
        }
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
