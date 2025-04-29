
using System.Collections.Generic;
using UnityEngine;

public class Farmacy : Facility
{
    [SerializeField] private float Work;

    public float _Work => Work;
    public float _RequiredWork => 0.4f;
    public CityStorage.ResourceType _RequiredResource => CityStorage.ResourceType.EnergyHoney;
    public float _Cost => 1;
    public override int _Order => -1;
    public override string _SaveInfo 
    {
        get
        {
            string info = base._SaveInfo + $"Work({Work})";

            return info;
        }
        set => base._SaveInfo = value; 
    }

    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        Work = StaticTools.StringToFloat(parameters["Work"]);
    }

    public override float _Effectivity
    {
        get
        {
            if (Bears.Length < 1)
            {
                return 0;
            }

            float work = 0;
            foreach (Bear bear in Bears)
            {
                work += bear._Work;
            }

            return work * Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2)  * (City._Research.GetResearchLevel(CityResearch.ResearchType.Production) >= 2 ? 1.25f : 1);
        }
    }
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        float work = 0;
        foreach (Bear bear in Bears)
        {
            work += bear._Work;
        }

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2),  true);

        if (Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0, true));
        }

        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Production) >= 2)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Технологические внедрения", 1.25f, true));
        }

        return factors;
    }

    public override void HourPassed()
    {
        base.HourPassed();

        if (Bears.Length <= 0)
        {
            return;
        }

        if (Work > 0)
        {
            Work -= CityTime._DaySection * _Effectivity;
        }
        string name = $"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}";
        while (Work <= 0)
        {
            bool produce = false;
                    if (City._Storage._EnergyHoney >= 1)
                    {
                        City._CityStatistics.AddStatistic(new CityStatistics.Statistic(name, (int)(City._Time._WorldTime / 60), -1, CityStorage.ResourceType.EnergyHoney));
                        City._CityStatistics.AddStatistic(new CityStatistics.Statistic(name, (int)(City._Time._WorldTime / 60), 1, CityStorage.ResourceType.Antisleep));
                        City._Storage._EnergyHoney -= 1;
                        City._Storage._Antisleep++;

                        produce = true;
                    }

            if (produce)
            {
                Work += _RequiredWork;
            }
            else
            {
                Work = 0;
                break;
            }
        }
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

        UserInteract.AskVariants($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", variants, indexes, RightMouseActions);
    }
    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {ConstructInfo.Name}" +
             $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
            $"\nПроизводимый препарат: Антиспячкин" +
            $"\nТребуемый материал: {CityStorage.ResourceName(_RequiredResource)} ({_Cost} ед)" +
            $"\nЭффективность: {(int)(_Effectivity * 100)}%" +
            $"\nСовершается работа: {(int)(_Effectivity * 100 * CityTime._DaySection)} %/ч" +
            $"\nОсталось работы: {(int)(_Work * 100)}%" +
            $"\n//////////";
    }
}
