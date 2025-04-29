
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnergyProcuder : Facility
{
    [SerializeField] private CityStorage.ResourceType ConsumeResource;
    [SerializeField] private float MaxLimit;
    [SerializeField] private float ConsumeLimit;
    [SerializeField] private float WorkPerLimit;
    [SerializeField] private float EnergyCapacity;

    [SerializeField] private GameObject HeatRangeIndicator;

    [SerializeField] private LayerMask FacilityLayer;
    [SerializeField] private int HeatEffect;
    [SerializeField] private float HeatRange;

    public CityStorage.ResourceType _Resource => ConsumeResource;
    public float _ConsumeLimit
    {
        get
        {
            return ConsumeLimit;
        }
        set
        {
            ConsumeLimit = Mathf.Clamp(value, 0, MaxLimit);
            SmtChanged();
        }
    }
    public float _BaseProduce => ConsumeLimit * ResourceEnergetic(ConsumeResource) ;
    public float _Producing
    {
        get
        {
            if (Bears.Length < 1)
            {
                return 0;
            }

            if (_Effectivity == 0)
            {
                return 0;
            }

            float value = 0;
            switch (ConsumeResource)
            {
                case CityStorage.ResourceType.Wood:
                    value = 3 * Mathf.Clamp01(City._Storage._Wood / ConsumeLimit) * ConsumeLimit / 2f;
                    value += value * _Effectivity * _Effectivity;
                    break;
                case CityStorage.ResourceType.Berezenium:
                    value = 8 * Mathf.Clamp01(City._Storage._Berezenium / ConsumeLimit) * ConsumeLimit  / 2f;
                    value += value * _Effectivity * _Effectivity;
                    break;
                case CityStorage.ResourceType.EnergyHoney:
                    value = 10 * Mathf.Clamp01(City._Storage._EnergyHoney / ConsumeLimit) * ConsumeLimit / 2f;
                    value += value * _Effectivity * _Effectivity;
                    break;
            }

            return value;
        }
    }
    public float _RequiredWork => Mathf.Max(1, ConsumeLimit * WorkPerLimit);
    public float _EnergyCapacity => EnergyCapacity;
    public override float _Effectivity
    {
        get
        {
            if(Bears.Length < 1)
            {
                return 0;
            }

            float work = 0;
            foreach (Bear bear in Bears)
            {
                work += bear._Work;
            }

            return Mathf.Clamp(work / _RequiredWork, 0, 1.25f)  * (City._Research.GetResearchLevel(CityResearch.ResearchType.Electricity) >= 2 ? 1.1f : 1);
        }
    }
    public override int _Order => -2;
    public override string _SaveInfo { get => base._SaveInfo + $"Limit({ConsumeLimit})"; set => base._SaveInfo = value; }
    public float _HeatEffect => _Effectivity > 0 ? HeatEffect : 0;
    public float _HeatRadius => HeatRange;

    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        ConsumeLimit = StaticTools.StringToFloat(parameters["Limit"]);
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        float work = 0;
        foreach (Bear bear in Bears)
        {
            work += bear._Work;
        }

        factors[0] = new CityFactors.Factor($"Работа медведей", Mathf.Clamp(work / _RequiredWork, 0, 1.25f), true);

        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Electricity) >= 2)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Разработки в электросистеме", 1.1f, true));
        }

        if (Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0, true));
        }

        return factors;
    }

    public override void HourPassed()
    {
        base.HourPassed();

        if (Bears.Length < 1)
        {
            return;
        }

        if (_Effectivity == 0)
        {
            return;
        }

        City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), -ConsumeLimit, ConsumeResource));
        float value = 0;
        float res = 0;
        switch (ConsumeResource)
        {
            case CityStorage.ResourceType.Wood:
                res = Mathf.Clamp01(City._Storage._Wood / ConsumeLimit);
                value = 3 * res * ConsumeLimit / 2f;
                value += value * _Effectivity * _Effectivity;

                City._Energosystem._CurrentEnergy += value;
                City._Storage._Wood -= ConsumeLimit ;
                break;
            case CityStorage.ResourceType.Berezenium:
                res = Mathf.Clamp01(City._Storage._Berezenium / ConsumeLimit);
                value = 8 * res * ConsumeLimit / 2f;
                value += value * _Effectivity * _Effectivity;

                City._Energosystem._CurrentEnergy += value;
                City._Storage._Berezenium -= ConsumeLimit ;
                break;
            case CityStorage.ResourceType.EnergyHoney:
                res = Mathf.Clamp01(City._Storage._EnergyHoney / ConsumeLimit);
                value = 10 * res * ConsumeLimit / 2f;
                value += value * _Effectivity * _Effectivity;
               
                City._Energosystem._CurrentEnergy += value;
                City._Storage._EnergyHoney -= ConsumeLimit;
                break;
        }

        City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), value, CityStorage.ResourceType.Electricity));

        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Cold) >= 2)
        {
            foreach (Collider collider in Physics.OverlapSphere(transform.position, HeatRange, FacilityLayer))
            {
                Facility facility = collider.GetComponentInParent<Facility>();
                if (facility != null && facility._CanBeHeated)
                {
                    facility._Heated += (int)(res * HeatEffect);
                }
            }
        }
    }

    public float ResourceEnergetic(CityStorage.ResourceType resource) 
    {
        switch (resource)
        {
            case CityStorage.ResourceType.Wood:
                return 3;
            case CityStorage.ResourceType.EnergyHoney:
                return 10;
            case CityStorage.ResourceType.Berezenium:
                return 8;
        }

        return 0;
    }

    private bool LowFuel()
    {
        switch (ConsumeResource)
        {
            case CityStorage.ResourceType.Wood:
                if(City._Storage._Wood < ConsumeLimit)
                {
                    return true;
                }
                break;
            case CityStorage.ResourceType.Berezenium:
                if (City._Storage._Berezenium < ConsumeLimit)
                {
                    return true;
                }
                break;
            case CityStorage.ResourceType.EnergyHoney:
                if (City._Storage._EnergyHoney < ConsumeLimit)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    public override void Interact()
    {
        SoundEffector.PlayFasilityIntro(IntroSound);

        string[] variants = new string[] { "Информация", "Снести" };
        int[] indexes = new int[] { -1, 0 };

        if (_CanBeHeated && City._Research.GetResearchLevel(CityResearch.ResearchType.Cold) >= 1)
        {
            variants = StaticTools.ExpandMassive(variants, "Установить режим работы обогревателя");
            indexes = StaticTools.ExpandMassive(indexes, 2);
        }

        if (_MaxBearCount > 0)
        {
            variants = StaticTools.ExpandMassive(variants, "Автоматически назначить");
            indexes = StaticTools.ExpandMassive(indexes, 3);

            if (AssignedBears.Length > 0)
            {
                variants = StaticTools.ExpandMassive(variants, "Снять всех с назначения");
                indexes = StaticTools.ExpandMassive(indexes, 4);
            }
        }

        variants = StaticTools.ExpandMassive(variants, "Установить лимит топлива");
        indexes = StaticTools.ExpandMassive(indexes, 5);

        UserInteract.AskVariants($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", variants, indexes, RightMouseActions);
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {ConstructInfo.Name}" +
             $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
            $"\nЛимит топлива: {(int)(_ConsumeLimit * 10) / 10f} ед/ч" +
            $"\nЭффективность: {(int)(_Effectivity * 100)}%" +
            $"\nЭнерговыработка: {Mathf.RoundToInt(_Producing)} ед/ч" +
            $"{(City._Research.GetResearchLevel(CityResearch.ResearchType.Cold) >= 2 ? $"Центральное отопление: {(_Effectivity > 0 ? HeatEffect : 0)}" : "")}" +
            $"{(LowFuel() ? $"\n<color=red>Мало топлива !</color>" : "")}" +
            $"\n/////////";
    }

    public override void RightMouseActions(int index)
    {
        base.RightMouseActions(index);

        if (index == 5)
        {
            UserInteract.AskInput($"Установить лимит\n[0; {MaxLimit}]", SetLimit)._Value = $"{ConsumeLimit}";
        }
    }
    public void SetLimit(string info)
    {
        _ConsumeLimit = StaticTools.StringToFloat(info);
    }

    public override void Indicate(bool state)
    {
        base.Indicate(state);

        if(City._Research.GetResearchLevel(CityResearch.ResearchType.Cold) >= 2)
        {
            HeatRangeIndicator.SetActive(state);
        }
    }
}
