using System.Collections.Generic;
using UnityEngine;

public class MedFacility : Facility
{
    [SerializeField] private int HealHours;
    [SerializeField] private int[] BearWork;

    public int _HealHours => HealHours;
    public int[] _BearWork => BearWork;

    public override int _Order => 3;

    public override string _SaveInfo 
    {
        get
        {
            string work = $"";
            foreach(int bearWork in BearWork)
            {
                work += $"{bearWork};";
            }

            if (work.EndsWith(";"))
            {
                work = work.Remove(work.Length - 1);
            }

            return base._SaveInfo + $"Work({work})";
        }
        set => base._SaveInfo = value; 
    }

    public override float _Effectivity
    {
        get
        {
            return Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2)  * (City._Research.GetResearchLevel(CityResearch.ResearchType.Medicine) >= 2 ? 1.5f : 1);
        }
    }

    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        string[] work = parameters["Work"].Split(";");
        BearWork = new int[work.Length];
        for(int i = 0; i < BearWork.Length; i++)
        {
            BearWork[i] = StaticTools.StringToInt(work[i]);
        }
        
        base.ApplySaveInfo(parameters);
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2), true);

        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Medicine) >= 2)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Качественная медицина", 1.5f, true));
        }

        if (Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0, true));
        }

        return factors;
    }

    public float AverageChance()
    {
        if(Bears.Length == 0)
        {
            return 0.2f;
        }

        float chance = 0;
        for (int i = 0; i < Bears.Length; i++)
        {
            chance += 20 * Bears[i]._Work * _Effectivity;
        }

        chance *= _Effectivity;

        chance /= Bears.Length;
        return chance;
    }

    public override void HourPassed()
    {
        base.HourPassed();

        if (Bears.Length < 1)
        {
            return;
        }

        Bear[] bears = City._DataBase._Bears;
        for (int i = 0; i < Bears.Length; i++)
        {
            if (Bears[i]._CurrentFacility == this)
            {
                BearWork[i]--;
                if (BearWork[i] <= 0)
                {
                    BearWork[i] = HealHours;

                    int minimal = 0;
                    for (int ii = 0; ii < bears.Length; ii++)
                    {
                        if (bears[ii]._Health < bears[minimal]._Health)
                        {
                            minimal = ii;
                        }
                    }

                    if (Random.Range(0, 101) <= 20 * Bears[i]._Work * _Effectivity)
                    {
                        bears[minimal]._Health++;
                    }
                }
            }
        }
    }

    public override bool AssignBear(Bear bear, bool remove)
    {
        if (remove)
        {
            int index = StaticTools.IndexOf(AssignedBears, bear);
            if (index >= 0)
            {
                BearWork = StaticTools.ReduceMassive(BearWork, index);
            }
        }
        else
        {
            if (bear._Sally != null)
            {
                return false;
            }

            BearWork = StaticTools.ExpandMassive(BearWork, HealHours);
        }

        return base.AssignBear(bear, remove);
    }

    public override void AutoAssign()
    {
        foreach (Bear bear in AssignedBears)
        {
            bear._Facility = null;
        }
        AssignedBears = new Bear[0];
        BearWork = new int[0];

        Bear[] bears = City._DataBase._Bears;

        while (AssignedBears.Length < _MaxBearCount)
        {
            int maximal = -1;
            for (int i = 0; i < bears.Length; i++)
            {
                if (bears[i]._Kasta == _RequiredKasta && bears[i]._Facility == null && !StaticTools.Contains(AssignedBears, bears[i]) && bears[i]._Sally == null)
                {
                    if (maximal == -1)
                    {
                        maximal = i;
                    }
                }
            }

            if (maximal == -1)
            {
                break;
            }

            AssignBear(bears[maximal], false);
        }

        SmtChanged();
    }

    public override void Unassign()
    {
        base.Unassign();

        BearWork = new int[0];
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {ConstructInfo.Name}" +
             $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
            $"\nВремя для лечения: {_HealHours} ч" +
            $"\nЭффективность: {(int)(_Effectivity * 100)}%" +
            $"\nСредний шанс лечения: {AverageChance()}%" +
            $"\n//////////";
    }
}
