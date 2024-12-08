
using System;
using UnityEngine;

public class ConstructionProject : Facility
{
    [SerializeField] private Constructor.ConstructInfo Construction;
    [SerializeField] private float WorkLeft;

    [SerializeField] private ResourceField ResourceField = null;

    public override bool _CanBeDisabled => false;

    public Constructor.ConstructInfo _Construction => Construction;
    public float _WorkLeft => WorkLeft;

    public void SetInfo(Constructor.ConstructInfo info, ResourceField field = null)
    {
        ResourceField = field;
        Construction = info;
        WorkLeft = info.BuildWork;
        RequiredSkill = info.BuildSkill;

        transform.localScale = new Vector3(info.Sizes.x, 10, info.Sizes.y);
    }

    public void DayPassed()
    {
        WorkLeft -= _Effectivity;

        if(WorkLeft <= 0)
        {
            City._Constructor.Build(Construction, new Vector3Int(Mathf.FloorToInt(transform.position.x), 0, Mathf.FloorToInt(transform.position.z)), Mathf.RoundToInt(transform.localEulerAngles.y / 90f), ResourceField);

            City._CityMessenger.SetMessage(new CityMessenger.CityMessage($"Постройка \"{Construction.Name}\" завершена", $"Здание наконец построено, пора укомплектовать строение."), false);

            Destroy(gameObject);
        }
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
            City._Storage._Wood += Construction.WoodCost;
            City._Storage._Metal += Construction.MetalCost;
            City._Storage._Berezenium += Construction.BerezenuimCost;
            Destroy(gameObject);
            City._Constructor.SpawnDemolishEffect(transform.position);
        }
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[2];

        float summSkill = 0;
        foreach (Bear bear in AssignedBears)
        {
            summSkill += bear._Skill;
        }

        factors[0] = new CityFactors.Factor($"Опыт медведей", Mathf.Clamp(summSkill / _RequiredSkill, 0.5f, 2), -1, true);
        factors[1] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, 0.5f, 1), -1, true);

        if (summSkill < _RequiredSkill)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Неопытность сотрудников", 0.5f, -1, true));
        }
        if (City._Buyiments._EffectivityBoost)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Промышленные нановнедрения", 1.25f, -1, true));
        }

        return factors;
    }
    public override float GetPotencialEffectivity()
    {
        float summSkill = 0;
        foreach (Bear bear in AssignedBears)
        {
            summSkill += bear.GetSkill(City._Foodstream._Potencial[2]);
        }

        return Mathf.Clamp(summSkill / _RequiredSkill, 0.5f, 1.5f) * Mathf.Clamp(Mathf.Pow(City._Energosystem._Potencial[2], 2), 0.5f, 1) * (summSkill < _RequiredSkill ? 0.5f : 1) * (City._Buyiments._EffectivityBoost ? 1.25f : 1);
    }
    public override float _Effectivity
    {
        get
        {
            float summSkill = 0;
            foreach (Bear bear in AssignedBears)
            {
                summSkill += bear._Skill;
            }

            return Mathf.Clamp(summSkill / _RequiredSkill, 0.5f, 2f) * Mathf.Clamp(City._Energosystem._Effectivity, 0.5f, 1) * (summSkill < _RequiredSkill ? 0.5f : 1) * (City._Buyiments._EffectivityBoost ? 1.25f : 1); 
        }
    }
    public override string _SaveInfo 
    { 
        get => base._SaveInfo + $"Construct({Construction.Prefab.name})Time({WorkLeft})Field({(ResourceField != null ? StaticTools.IndexOf(City._CityGeology._Fields, ResourceField) : "n")})";
        set
        {
            base._SaveInfo = value;
            WorkLeft = StaticTools.StringToFloat(StaticTools.GetParameter(value, "Time"));

            string field = StaticTools.GetParameter(value, "Field");
            if(field != "n")
            {
                ResourceField = City._CityGeology._Fields[StaticTools.StringToInt(field)];
            }
        }
    }
}
