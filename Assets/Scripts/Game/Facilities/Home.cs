using System.Collections.Generic;
using UnityEngine;

public class Home : Facility
{
    [SerializeField] private float StressDown;

    public float _BaseStressDown => StressDown;
    public float _StressDown => StressDown * _Effectivity;
    public override int _ColdEndurance => base._ColdEndurance + (City._Research.GetResearchLevel(CityResearch.ResearchType.Household) >= 2 ? 2 : 0);
    public override float _Effectivity => Mathf.Max(MinimalEnergyCoeffiente, City._Energosystem._Effectivity) ;

    protected override void OnDestroy()
    {
        City._DataBase.RegisterFacility(this, true);

        foreach (Bear bear in AssignedBears)
        {
            bear._Home = null;
        }

        SmtChanged();
    }

    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        string bearses = parameters["Bears"];

        _Heater = StaticTools.StringToInt(parameters["Heater"]);

        if (bearses.Length > 0)
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
    public override void AutoAssign()
    {
        foreach (Bear bear in AssignedBears)
        {
            bear._Home = null;
        }

        AssignedBears = new Bear[0];
        foreach (Bear bear in City._DataBase._Bears)
        {
            if (bear._Home == null && bear._Sally == null)
            {
                bear._Home = this;
                AssignedBears = StaticTools.ExpandMassive(AssignedBears, bear);

                if (AssignedBears.Length == _MaxBearCount)
                {
                    break;
                }
            }
        }

        SmtChanged();
    }
    public override void RightMouseActions(int index)
    {
        if(index == 3)
        {
            AutoAssign();
        }
        else if(index == 4)
        {
            Unassign();
        }
        else
        {
            base.RightMouseActions(index);
        }
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {ConstructInfo.Name}" +
             $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nОтдыхают: {Bears.Length}/{AssignedBears.Length}" +
            $"\n/////////";
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Max(MinimalEnergyCoeffiente, City._Energosystem._Effectivity), true);

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

                SmtChanged();

                return true;
            }
        }
        else
        {
            if (bear._Sally != null)
            {
                return false;
            }

            if (index < 0 && AssignedBears.Length < _MaxBearCount)
            {
                AssignedBears = StaticTools.ExpandMassive(AssignedBears, bear);

                if(bear._Home != null)
                {
                    bear._Home.AssignBear(bear, true);
                }

                bear._Home = this;

                SmtChanged();

                return true;
            }
        }

        return false;
    }

    public override void Unassign()
    {
        foreach (Bear bear in AssignedBears)
        {
            bear._Home = null;
        }

        AssignedBears = new Bear[0];

        SmtChanged();
    }
}