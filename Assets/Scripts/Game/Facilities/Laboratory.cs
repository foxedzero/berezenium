using System.Collections.Generic;
using UnityEngine;
using static CityResearch;

public class Laboratory : Facility
{
    [SerializeField] private CityResearch.ResearchType TargetResearch;
    [SerializeField] private float ResearchNominal;

    public CityResearch.ResearchType _Target
    {
        get
        {
            return TargetResearch;
        }
        set
        {
            TargetResearch = value;

            RequiredKasta = City._Research.ResearchKasta(TargetResearch);

            SmtChanged();
        }
    }
    public float _ResearchPoints
    {
        get
        {
            float value = 0;

            foreach(Bear bear in Bears)
            {
                value += bear._Work * ResearchNominal;
            }

            return value;
        }
    }
    public override float _Effectivity
    {
        get
        {
            if(Bears.Length <= 0)
            {
                return 0;
            }

            return Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2) ;
        }
    }

    public override string _SaveInfo 
    { 
        get => base._SaveInfo + $"Target({TargetResearch.GetHashCode()})";
        set
        {
            base._SaveInfo = value;
        }
    }

    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        _Target = (CityResearch.ResearchType)StaticTools.StringToInt(parameters["Target"]);
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2), true);

        if(Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0,  true));
        }

        return factors;
    }

    protected override void UpdateOcantovkaInfo()
    {
        string info = "";
        switch (TargetResearch)
        {
            case CityResearch.ResearchType.Electricity:
                info = $"Электроэнергия";
                break;
            case CityResearch.ResearchType.Cold:
                info = $"Отопительные системы";
                break;
            case CityResearch.ResearchType.Medicine:
                info = $"Медицина и препараты";
                break;
            case CityResearch.ResearchType.Travels:
                info = $"Путешествия и логистика";
                break;
            case CityResearch.ResearchType.Household:
                info = $"Жилищные условия";
                break;
            case CityResearch.ResearchType.Food:
                info = $"Пасеки и мёд";
                break;
            case CityResearch.ResearchType.Production:
                info = $"Производство";
                break;
            case CityResearch.ResearchType.Mining:
                info = $"Добыча";
                break;
        }
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {ConstructInfo.Name}" +
             $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
            $"\nИсследование: {info}" +
            $"\nЭффективность: {(int)(_Effectivity * 100)}%" +
            $"\nИзучение: {(int)(_ResearchPoints * _Effectivity )} ед/ч" +
            $"{(City._Energosystem._Effectivity == 0 ? "\n<color=red>Нет электричества !</color>" : "")}" +
            $"\n/////////";
    }

    public override void HourPassed()
    {
        base.HourPassed();

        if (Bears.Length < 1)
        {
            return;
        }

        City._Research.Research(TargetResearch, _ResearchPoints * _Effectivity);
    }
}
