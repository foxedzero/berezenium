using UnityEngine;

public class ExsaustProducer : Producer
{
    [SerializeField] private ResourceField ResourceField;

    public override event SimpleVoid OnSmthChange = null;
    public override int _ColdEndurance => base._ColdEndurance + (City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 1 ? 3 : 0);
    public override float _Effectivity => ResourceField._Count > 0 ? base._Effectivity : 0;
    public override int _RequiredRobots => ResourceField._Count > 0 ? base._RequiredRobots : 0;
    public override int _MaxBearCount => ResourceField._Count > 0 ? base._MaxBearCount : 0;
    public float _ResourcesLeft => ResourceField._Count;

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

    public override int _Robots 
    { 
        get => base._Robots;
        set
        {
            base._Robots = value;

            if (OnSmthChange != null)
            {
                OnSmthChange.Invoke();
            }
        }
    }

    public override string _SaveInfo
    {
        get
        {
            return base._SaveInfo + $"Field({StaticTools.IndexOf( City._CityGeology._Fields, ResourceField)})";
        }
        set
        {
            base._SaveInfo = value;

            ResourceField = City._CityGeology._Fields[StaticTools.StringToInt(StaticTools.GetParameter(value, "Field"))];

            transform.localRotation = ResourceField.transform.localRotation;    
        }
    }

    public override float GetPotencialEffectivity() => ResourceField._Count > 0 ? base.GetPotencialEffectivity() : 0;
    public override CityFactors.Factor[] GetEffectivityFactors() => new CityFactors.Factor[] { new CityFactors.Factor("Запасы истощены", 0, -1, true)};

    public override void DayPassed()
    {
        switch (Resource)
        {
            case CityStorage.ResourceType.EnergyHoney:
                City._Storage._EnergyHoney += Mathf.Max(0, Mathf.Min(NominalCount  * _Effectivity, ResourceField._Count)) *(City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 2 ? 1.25f : 1);
                break;
            case CityStorage.ResourceType.Berezenium:
                City._Storage._Berezenium += Mathf.Max(0, Mathf.Min(NominalCount * _Effectivity, ResourceField._Count)) * (City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 2 ? 1.25f : 1);
                break;
            case CityStorage.ResourceType.Metal:
                City._Storage._Metal += Mathf.Max(0, Mathf.Min(NominalCount * _Effectivity, ResourceField._Count)) * (City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 2 ? 1.25f : 1); 
                break;
            case CityStorage.ResourceType.Wood:
                City._Storage._Wood += Mathf.Max(0, Mathf.Min(NominalCount * _Effectivity, ResourceField._Count)) * (City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 2 ? 1.25f : 1); 
                break;
            case CityStorage.ResourceType.Robots:
                City._Storage._Robots += Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(NominalCount * _Effectivity, ResourceField._Count)));
                break;
        }

        if (Resource == CityStorage.ResourceType.Robots)
        {
            ResourceField._Count -= Mathf.Max(0, Mathf.FloorToInt(NominalCount * _Effectivity));
        }
        else
        {
            ResourceField._Count -= Mathf.Max(0, NominalCount * _Effectivity);
        }

        if(ResourceField._Count <= 0)
        {
            City._CityMessenger.SetMessage(new CityMessenger.CityMessage($"{ConstructInfo.Name} истощён", $"{ConstructInfo.Name} истощился, теперь там нечего добывать."), false);

            foreach(Bear bear in AssignedBears)
            {
                bear._Facility = null;
            }
            City._Storage._Robots += Robots;
            Robots = 0;

            AssignedBears = new Bear[0];
        }

            OnSmthChange?.Invoke();
    }

    public void SetField(ResourceField field)
    {
        if(field._ResourceType != _ResourceType)
        {
            return;
        }

        ResourceField = field;
        transform.localRotation = ResourceField.transform.localRotation;
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
