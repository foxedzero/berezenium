using UnityEngine;
using System.Collections.Generic;

public class Factory : Facility
{
    [SerializeField] private CityStorage.ResourceType ResourceConsume;
    [SerializeField] private CityStorage.ResourceType ResourceProduce;
    [SerializeField] private float Work;
    [SerializeField] private bool MechFactory;

    public CityStorage.ResourceType _RequiredResource => ResourceConsume;
    public CityStorage.ResourceType _ResourceProduce => ResourceProduce;
    public float _RequiredWork
    {
        get
        {
            switch (ResourceProduce)
            {
                case CityStorage.ResourceType.Robots:
                    return 0.75f;
                case CityStorage.ResourceType.CyberBee:
                    return 0.85f;
                case CityStorage.ResourceType.Snowrunners:
                    return 1f;
                case CityStorage.ResourceType.EnergyHoney:
                    return 0.021f;
                case CityStorage.ResourceType.Berezenium:
                    return 0.035f;
            }

            return 1;
        }
    }
    public float _Cost
    {
        get
        {
            switch (ResourceProduce)
            {
                case CityStorage.ResourceType.Robots:
                    return 6;
                case CityStorage.ResourceType.CyberBee:
                    return 8;
                case CityStorage.ResourceType.Snowrunners:
                    return 8;
                case CityStorage.ResourceType.EnergyHoney:
                    return 3;
                case CityStorage.ResourceType.Berezenium:
                    return 0.5f;
            }

            return 1;
        }
    }
    public float _Work => Work;
    public bool _MechFactory => MechFactory;

    public override string _SaveInfo { get => base._SaveInfo + $"Work({Work}){(MechFactory ? $"Target({ResourceProduce.GetHashCode()})" : "")}"; set => base._SaveInfo = value; }
    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        if (MechFactory)
        {
            ResourceProduce = (CityStorage.ResourceType)StaticTools.StringToInt(parameters["Target"]);
        }
        Work = StaticTools.StringToFloat(parameters["Work"]);
    }

    public override float _Effectivity
    {
        get
        {
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
        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Production) >= 2)
        {
            return StaticTools.ExpandMassive(base.GetEffectivityFactors(), new CityFactors.Factor("Технологические внедрения", 1.25f, true));
        }
        else
        {
            return base.GetEffectivityFactors();
        }
    }

    public override void HourPassed()
    {
        if (Bears.Length <= 0)
        {
            return;
        }

        switch (HeaterOn)
        {
            case 1:
                if (City._Storage._EnergyHoney >= 1)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Обогреватель {ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), -1, CityStorage.ResourceType.EnergyHoney));
                    City._Storage._EnergyHoney -= 1;
                    Heated += 3;
                }
                break;
            case 2:
                if (City._Storage._Wood >= 5)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Обогреватель {ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), -5, CityStorage.ResourceType.Wood));
                    City._Storage._Wood -= 5;
                    Heated += 2;
                }
                break;
        }

        if(Work > 0)
        {
            foreach (Bear bear in Bears)
            {
                bear._Tired += CityTime._DaySection * Tiring * bear._TiredCoefficient;
            }

            Work -= CityTime._DaySection * _Effectivity;
        }
        string name = $"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}";

        float usedResources = 0;
        float produced = 0;
        while (Work <= 0)
        {
            if (City._Storage.GetResource(ResourceConsume) >= _Cost)
            {
                usedResources += _Cost;
                produced += 1;
                City._Storage.AddResource(ResourceConsume, -_Cost);
                City._Storage.AddResource(ResourceProduce, 1);
                Work += _RequiredWork;
            }
            else
            {
                Work = 0;
                break;
            }
        }
        City._CityStatistics.AddStatistic(new CityStatistics.Statistic(name, (int)(City._Time._WorldTime / 60), -usedResources, ResourceConsume));
        City._CityStatistics.AddStatistic(new CityStatistics.Statistic(name, (int)(City._Time._WorldTime / 60), produced, ResourceProduce));
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

        if (_MechFactory)
        {
            variants = StaticTools.ExpandMassive(variants, "Изменить производство");
            indexes = StaticTools.ExpandMassive(indexes, 5);
        }

        UserInteract.AskVariants($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", variants, indexes, RightMouseActions);
    }
    public override void RightMouseActions(int index)
    {
        base.RightMouseActions(index);

        if(_MechFactory && index == 5)
        {
            string[] variants = new string[] { "Роботы" };
            int[] indexes = new int[] { 0 };

            if(City._Research.GetResearchLevel(CityResearch.ResearchType.Travels) >= 2)
            {
                variants = StaticTools.ExpandMassive(variants, "Снегоходы");
                indexes = StaticTools.ExpandMassive(indexes, 1);
            }

            if (City._Research.GetResearchLevel(CityResearch.ResearchType.Food) >= 3)
            {
                variants = StaticTools.ExpandMassive(variants, "Киберпчёлы");
                indexes = StaticTools.ExpandMassive(indexes, 2);
            }

            UserInteract.AskVariants("Изменить производство", variants, indexes, SetProduce);
        }
    }
    public void SetProduce(int index)
    {
        switch (index)
        {
            case 0:
                if(ResourceProduce == CityStorage.ResourceType.Robots)
                {
                    return;
                }
                ResourceProduce = CityStorage.ResourceType.Robots;
                break;
            case 1:
                if (ResourceProduce == CityStorage.ResourceType.Snowrunners)
                {
                    return;
                }
                ResourceProduce = CityStorage.ResourceType.Snowrunners;
                break;
            case 2:
                if (ResourceProduce == CityStorage.ResourceType.CyberBee)
                {
                    return;
                }
                ResourceProduce = CityStorage.ResourceType.CyberBee;
                break;
        }

        Work = _RequiredWork;
        SmtChanged();
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {ConstructInfo.Name}" +
             $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
            $"\nПроизводимый ресурс: {CityStorage.ResourceName(_ResourceProduce)}" +
            $"\nТребуемый материал: {CityStorage.ResourceName(_RequiredResource)} ({_Cost} ед)" +
            $"\nЭффективность: {(int)(_Effectivity * 100)}%" +
            $"\nСовершается работа: {(int)(_Effectivity * 100 * CityTime._DaySection)} %/ч" +
            $"\nОсталось работы: {(int)(_Work * 100)}%" +
            $"{(City._Energosystem._Effectivity == 0 ? "\n<color=red>Нет электричества !</color>" : "")}" +
            $"\n//////////";
    }

}
