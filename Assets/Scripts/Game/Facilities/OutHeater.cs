using System.Collections.Generic;
using UnityEngine;

public class OutHeater : Facility
{
    [SerializeField] private GameObject HeatRangeIndicator;

    [SerializeField] private LayerMask Layer;
    [SerializeField] private float Range;
    [SerializeField] private int HeatLevel;

    public override int _Order => -3;

    public int _HeatLevel
    {
        get
        {
            if (_Consume > City._Storage._EnergyHoney)
            {
                return 0;
            }

            return HeatLevel;
        }
        set
        {
            HeatLevel = Mathf.Clamp(value, 0, 5);
        }
    }
    public float _HeatRadius => Range;
    public float _Consume
    {
        get
        {
            float value = 0;
            for(int i = 1; i <= HeatLevel; i++)
            {
                value += i;
            }
            return value;
        }
    }
    public override bool _CanBeHeated => false;
    public override float _Effectivity => 1;

    public override string _SaveInfo { get => base._SaveInfo + $"Nominal({HeatLevel})"; set => base._SaveInfo = value; }

    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        HeatLevel = StaticTools.StringToInt(parameters["Nominal"]);
    }

    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        return new CityFactors.Factor[] { new CityFactors.Factor("Автономное строение", 1, true) };
    }


    public override void HourPassed()
    {
        if(HeatLevel == 0)
        {
            return;
        }

        if(_Consume > City._Storage._EnergyHoney)
        {
            return;
        }

        City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), -_Consume, CityStorage.ResourceType.EnergyHoney));
        City._Storage._EnergyHoney -= _Consume;

        foreach (Collider collider in Physics.OverlapSphere(transform.position, Range, Layer))
        {
            Facility facility = collider.GetComponentInParent<Facility>();
            if (facility != null && facility._CanBeHeated)
            {
                facility._Heated += HeatLevel;
            }
        }
    }

    public override void Indicate(bool state)
    {
        base.Indicate(state);

        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Cold) >= 2)
        {
            HeatRangeIndicator.SetActive(state);
        }
    }

    public void SetHeatNominal(string info)
    {
        HeatLevel = Mathf.Clamp(StaticTools.StringToInt(info), 0, 5);
        SmtChanged();
    }
    public override void RightMouseActions(int index)
    {
        base.RightMouseActions(index);

        if(index == 5)
        {
            UserInteract.AskInput("Номинал отопления", SetHeatNominal)._Value = $"{HeatLevel}";
        }
    }
    public override void Interact()
    {
        SoundEffector.PlayFasilityIntro(IntroSound);

        string[] variants = new string[] { "Информация", "Снести" };
        int[] indexes = new int[] { -1, 0 };

        variants = StaticTools.ExpandMassive(variants, "Установить номинал отопления");
        indexes = StaticTools.ExpandMassive(indexes, 5);

        UserInteract.AskVariants($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", variants, indexes, RightMouseActions);
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////\nОбъект: {ConstructInfo.Name}\nПотребление энергомёда: {_Consume}\nНоминал отопления: {HeatLevel}\nАвтономное строение\n/////////";
    }
}
