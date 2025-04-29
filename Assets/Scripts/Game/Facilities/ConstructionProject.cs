
using System.Collections.Generic;
using UnityEngine;
using static Constructor;

public class ConstructionProject : Facility
{
    [SerializeField] private Constructor.ConstructInfo Construction;
    [SerializeField] private Transform Box;
    [SerializeField] private float WorkLeft;

    [SerializeField] private ResourceField ResourceField = null;

    public Constructor.ConstructInfo _Construction => Construction;
    public float _WorkLeft
    {
        get
        {
            return WorkLeft;
        }
        set
        {
            WorkLeft = value;
            if (WorkLeft <= 0)
            {
                Facility newFacility = City._Constructor.Build(Construction, new Vector3Int(Mathf.FloorToInt(transform.position.x), 0, Mathf.FloorToInt(transform.position.z)), Mathf.RoundToInt(transform.localEulerAngles.y / 90f), ResourceField);

                City._CityMessenger.AddMessage(new CityMessenger.CityMessage($"Постройка \"{Construction.Name}\" завершена", $"Здание наконец построено, пора укомплектовать персонал."));

                foreach (Bear bear in Bears)
                {
                    bear._CurrentFacility = newFacility;
                }

                Destroy(gameObject);
            }

            SmtChanged();
        }
    }

    public void SetInfo(Constructor.ConstructInfo info, ResourceField field = null)
    {
        ResourceField = field;
        Construction = info;
        WorkLeft = info.BuildWork;

        Box.localScale = new Vector3(info.Sizes.x, 10, info.Sizes.y);
        Box.position += Vector3.up * 5;

        _EnterPoint.position = new Vector3(transform.position.x, 0, transform.position.z + (info.Sizes.y / 2f) + 1);

        if (WorkLeft <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void HourPassed()
    {
        base.HourPassed();

        if (Bears.Length < 1)
        {
            return;
        }

        WorkLeft -= CityTime._DaySection * _Effectivity;

        if (WorkLeft <= 0)
        {
            Vector3Int position = new Vector3Int(Mathf.FloorToInt(transform.position.x), 0, Mathf.FloorToInt(transform.position.z));
            if (Physics.BoxCast(position + Vector3.up * 50, new Vector3(Construction.Sizes.x / 2, 0.01f, Construction.Sizes.y / 2), Vector3.down, out RaycastHit hit22, transform.rotation, 100, 128))
            {
                position.y = Mathf.RoundToInt(hit22.point.y);
            }

            Facility newFacility = City._Constructor.Build(Construction, position, Mathf.RoundToInt(transform.localEulerAngles.y / 90f), ResourceField);

            City._CityMessenger.AddMessage(new CityMessenger.CityMessage($"Постройка \"{Construction.Name}\" завершена", $"Здание наконец построено, пора укомплектовать персонал."));

            foreach(Bear bear in Bears)
            {
                bear._CurrentFacility = newFacility;
            }

            Destroy(gameObject);
        }

        SmtChanged();
    }

    public override void RightMouseActions(int index)
    {
        if (index == 0)
        {
            UserInteract.AskConfirm("Отмена строительства", $"Вы собираетесь дать распоряженио об отмене строительства данного строения.\nПри отмене вы вернёте ресурсы, затраченные на строительство (древесина: {Construction.WoodCost}, металл: {Construction.MetalCost}, березениум: {ConstructInfo.BerezenuimCost}).\nВы уверены ?", Deconstruct);
        }
        else
        {
            base.RightMouseActions(index);
        }
    }
    public override void Deconstruct(bool answer)
    {
        if (answer)
        {
            if(Construction.WoodCost > 0)
            {
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"отмена проекта {Construction.Name}", (int)(City._Time._WorldTime / 60), Construction.WoodCost, CityStorage.ResourceType.Wood));
                City._Storage._Wood += Construction.WoodCost;
            }
            if (Construction.MetalCost > 0)
            {
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"отмена проекта {Construction.Name}", (int)(City._Time._WorldTime / 60), Construction.MetalCost, CityStorage.ResourceType.Metal));
                City._Storage._Metal += Construction.MetalCost;
            }
            if (Construction.BerezenuimCost > 0)
            {
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"отмена проекта {Construction.Name}", (int)(City._Time._WorldTime / 60), Construction.BerezenuimCost, CityStorage.ResourceType.Berezenium));
                City._Storage._Berezenium += Construction.BerezenuimCost;
            }

            Destroy(gameObject);
            City._Constructor.SpawnDemolishEffect(transform.position);
        }
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {Construction.Name}" +
             $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
            $"\nЭффективность: {(int)(_Effectivity * 100)}%" +
            $"\nСовершается работа: {(int)(_Effectivity * 100  * CityTime._DaySection)} %/ч" +
            $"\nОсталось работы: {(int)(_WorkLeft * 100)}%" +
            $"\n/////////";
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        float work = 0;
        foreach (Bear bear in Bears)
        {
            work += bear._Work;
        }

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Max(1, Mathf.Clamp(City._Energosystem._Effectivity, 0, 1) * 1.5f), true);

        if (Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0, true));
        }

        return factors;
    }

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

            return work *  Mathf.Max(1, Mathf.Clamp(City._Energosystem._Effectivity, 0, 1) * 1.5f) ; 
        }
    }
    public override string _SaveInfo 
    { 
        get => base._SaveInfo + $"Construct({Construction.Prefab.name})Time({WorkLeft})Field({(ResourceField != null ? StaticTools.IndexOf(City._CityGeology._Fields, ResourceField) : "n")})";
        set
        {
            base._SaveInfo = value;
        }
    }
    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        WorkLeft = StaticTools.StringToFloat(parameters["Time"]);

        string field = parameters["Field"];
        if (field != "n")
        {
            ResourceField = City._CityGeology._Fields[StaticTools.StringToInt(field)];
        }
    }
}
