
using System.Collections.Generic;
using UnityEngine;
using static Constructor;

public class Assimilator : Facility
{
    [SerializeField] protected int Robots;
    [SerializeField] protected int MaxRobots;
    [SerializeField] private float RequiredRobotWork;

    [SerializeField] protected float[] TireCoefficient;

    [SerializeField] protected bool UseRobots; //ручной, роботизированый

    public override int _ColdEndurance => base._ColdEndurance + (City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 1 ? 3 : 0);
	public override float _Effectivity
    {
        get
        {
            if (UseRobots)
            {
                float work = 0;
                foreach (Bear bear in Bears)
                {
                    work += bear._Work;
                }

                return Mathf.Clamp(work / _RequiredWork, 0, 1.25f) * Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2) ;
            }
            else
            {
                if(Bears.Length < 1)
                {
                    return 0;
                }

                float work = 0;
                foreach(Bear bear in Bears)
                {
                    work += bear._Work;
                }

                return work  * (City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 1 ? 1.1f : 1) * (City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 3 ? 1.25f : 1);
            }
        }
    }
    public virtual int _MaxRobots =>  MaxRobots;

    public float _RequiredWork => Robots == 0 ? RequiredRobotWork : RequiredRobotWork * Robots;

    public override int _Order => 2;

    public int _Robots
    {
        get
        {
            return Robots;
        }
        set
        {
            Robots = value;
            SmtChanged();
        }
    }

    public override string _SaveInfo
    {
        get
        {
            return base._SaveInfo + $"UseRobots({UseRobots.GetHashCode()})Robots({Robots})";
        }
        set
        {
            base._SaveInfo = value;
        }
    }
    public bool _UseRobots
    {
        get
        {
            return UseRobots;
        }
        set
        {
            if(value == false && ConstructInfo.MiningResource == CityStorage.ResourceType.Berezenium)
            {
                UserInteract.AskMessage("Ручной труд невозможен !", "Березениум - опасный минерал, следует использовать роботизированный труд.");
                value = true;
            }

            UseRobots = value;

            if (UseRobots == false)
            {
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), Robots, CityStorage.ResourceType.Robots));

                City._Storage._Robots += Robots;
                Robots = 0;

                RequiredKasta = Bear.Kasta.Неопределено;

                Tiring = TireCoefficient[0];
            }
            else
            {
                RequiredKasta = Bear.Kasta.Программист;

                Tiring = TireCoefficient[1];
            }

            SmtChanged();
        }

    }

    public float _Producing
    {
        get
        {
            if(_Effectivity == 0)
            {
                return 0;
            }

            float value = 0;
            if (UseRobots)
            {
                value = GetNominal() / 2f;
                value += value * _Effectivity;
            }
            else
            {
                value = GetNominal() * _Effectivity;
            }

            return value;
        }
    }

    public override float _EnergyConsume => Bears.Length == 0 ? 0 : (UseRobots ? RequiredEnergy * 2 : RequiredEnergy);
    public override float _BaseEnergyConsume => (UseRobots ? RequiredEnergy * 2 : RequiredEnergy);

    protected override void OnDestroy()
    {
        base.OnDestroy();

        City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), Robots, CityStorage.ResourceType.Robots));
        City._Storage._Robots += Robots;
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[0];

        float work = 0;
        foreach (Bear bear in Bears)
        {
            work += bear._Work;
        }

        if (UseRobots)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Работа/Требуемая", work / _RequiredWork, true));
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2), true));
        }

        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 1)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Модернизированные инструменты", 1.25f, true));
        }
        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Mining) >= 3)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Березениумные инструменты", 1.25f, true));
        }

        if (Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0, true));
        }

        return factors;
    }

    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        Robots = StaticTools.StringToInt(parameters["Robots"]);
        _UseRobots = parameters["UseRobots"] == "1";
    }

    public virtual float GetNominal()
    {
        float nominal = 0;
        switch (ConstructInfo.MiningResource)
        {
            case CityStorage.ResourceType.Berezenium:
                nominal = 0.7f;
                break;
            case CityStorage.ResourceType.Metal:
                nominal = 1;
                break;
            case CityStorage.ResourceType.Wood:
                nominal = 3f;
                break;
        }

        if (UseRobots)
        {
            nominal *= Robots;
        }

        return nominal;
    }

    public override void HourPassed()
    {
        base.HourPassed();

        if(AssignedBears.Length < 1)
        {
            return;
        }

        if (_Effectivity == 0)
        {
            return;
        }

        float value = 0;
        if (UseRobots)
        {
            value = GetNominal() / 2f;
            value += value * _Effectivity;
        }
        else
        {
            value = GetNominal() * _Effectivity;
        }

        City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), value, ConstructInfo.MiningResource));
        
        City._Storage.AddResource(ConstructInfo.MiningResource, value);
    }

    protected override void UpdateOcantovkaInfo()
    {
        if (UseRobots)
        {
            OcantovkaInfo.text = $"/////////" +
                $"\nОбъект: {ConstructInfo.Name}" +
                 $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
                $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
                $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
                $"\nРоботы: {Robots}/{MaxRobots}" +
                $"\nДобываемый ресурс: {CityStorage.ResourceName(_ConstructInfo.MiningResource)}" +
                $"\nЭффективность: {(int)(_Effectivity * 100)}%" +
                $"\nОбъем добычи: {Mathf.RoundToInt(_Producing)} ед/ч" +
                $"{(Robots == 0 ? "\n<color=red>Отсутствуют роботы !</color>" : "")}" +
                $"\n//////////";
        }
        else
        {
            OcantovkaInfo.text = $"/////////" +
                $"\nОбъект: {ConstructInfo.Name}" +
                 $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
                $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
                $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
                $"\nДобываемый ресурс: {CityStorage.ResourceName(_ConstructInfo.MiningResource)}" +
                $"\nЭффективность: {(int)(_Effectivity * 100)}%" +
                $"\nОбъем добычи: {Mathf.RoundToInt(_Producing)} ед/ч" +
                $"\n//////////";
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

        variants = StaticTools.ExpandMassive(variants, UseRobots ? "Сменить режим работы на ручной" : "Сменить режим работы на роботизированный");
        indexes = StaticTools.ExpandMassive(indexes, 5);

        UserInteract.AskVariants($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", variants, indexes, RightMouseActions);
    }

    public override void RightMouseActions(int index)
    {
        base.RightMouseActions(index);

        if (index == 5)
        {
            UserInteract.AskVariants($"Режим работы", new string[] {"Ручной", "Роботизированный"}, new int[] {0, 1}, SetRegim);
        }
    }
    public void SetRegim(int index)
    {
        _UseRobots = index == 1;
    }
}
