using UnityEngine;

public class Home : Facility
{
    [SerializeField] private int SatisfactionBonus;

    public override bool _CanBeDisabled => false;

    public override event SimpleVoid OnSmthChange = null;

    public override string _SaveInfo 
    { 
        get => base._SaveInfo;
        set
        {
            string bearses = StaticTools.GetParameter(value, "Bears");

            if(bearses.Length > 0)
            {
                string[] bears = bearses.Split(";");
                AssignedBears = new Bear[bears.Length];

                for (int i = 0; i < bears.Length; i++)
                {
                    AssignedBears[i] = City._DataBase._Bears[int.Parse(bears[i])];
                    AssignedBears[i]._Home = this;
                }
            }
        } 
    }
    public override int _ColdEndurance => base._ColdEndurance + (City._Research.GetResearchLevel(CityResearch.ResearchType.Household) >= 2 ? 3 : 0);
    public int _SatisfactionBonus => SatisfactionBonus;
    public override float _Effectivity => City._Energosystem._Effectivity * (City._Buyiments._EffectivityBoost ? 1.25f : 0);

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

    protected override void OnDestroy()
    {
        City._DataBase.RegisterFacility(this, true);

        foreach (Bear bear in AssignedBears)
        {
            bear._Home = null;
        }

        if (OnSmthChange != null)
        {
            OnSmthChange.Invoke();
        }
    }

    public override void RightMouseActions(int index)
    {
        if(index == 3)
        {
            foreach (Bear bear in AssignedBears)
            {
                bear._Home = null;
            }

            AssignedBears = new Bear[0];
            foreach(Bear bear in City._DataBase._Bears)
            {
                if(bear._Home == null)
                {
                    bear._Home = this;
                    AssignedBears = StaticTools.ExpandMassive(AssignedBears, bear);

                    if(AssignedBears.Length == _MaxBearCount)
                    {
                        break;
                    }
                }
            }

            OnSmthChange?.Invoke();
        }
        else if(index == 4)
        {
            foreach (Bear bear in AssignedBears)
            {
                bear._Home = null;
            }

            AssignedBears = new Bear[0];

            OnSmthChange?.Invoke();
        }
        else
        {
            base.RightMouseActions(index);
        }
    }

    public override float GetPotencialEffectivity()
    {
        return Mathf.Pow(City._Energosystem._Potencial[2], 2) * (City._Buyiments._EffectivityBoost ? 1.25f : 0);
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", City._Energosystem._Effectivity, -1, true);

        if (City._Buyiments._EffectivityBoost)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Промышленные нановнедрения", 1.25f, -1, true));
        }

        return factors;
    }

    public override bool AssignBear(Bear bear, bool remove)
    {
        int index = StaticTools.IndexOf(AssignedBears, bear);
        if (remove)
        {
            if (index > -1)
            {
                AssignedBears = StaticTools.ReduceMassive(AssignedBears, index);

                bear._Home = null;

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

                if(bear._Home != null)
                {
                    bear._Home.AssignBear(bear, true);
                }

                bear._Home = this;

                if (OnSmthChange != null)
                {
                    OnSmthChange.Invoke();
                }

                return true;
            }
        }

        return false;
    }

    public override void SetHeater(int index)
    {
        base.SetHeater(index);
        OnSmthChange?.Invoke();
    }
    public override void GiveExperience() { }
}