
using UnityEngine;

public class RobotFactory : Facility
{
    [SerializeField] private float BaseProduce;

    public int _RobotCost => 5 - (City._Research.GetResearchLevel(CityResearch.ResearchType.Production) >= 2 ? 2 : 0);
    public override float _Effectivity => base._Effectivity * (Enabled ? 1 : 0);
    public override float GetPotencialEffectivity()
    {
        return base.GetPotencialEffectivity() * (Enabled ? 1 : 0);
    }
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = base.GetEffectivityFactors();
        if (!Enabled)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Работа приостановлена", 0, -1, true));
        }
        return factors;
    }

    public float _BaseProduce => BaseProduce;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        City._Time.DayChanged -= DayPassed;
    }

    protected override void Start()
    {
        City._Time.DayChanged += DayPassed;
    }

    public void DayPassed()
    {
        if (!Enabled)
        {
            return;
        }

        int done = Mathf.Max(0, Mathf.FloorToInt(BaseProduce * _Effectivity));
        if(done * 5 > City._Storage._Metal)
        {
            while(done > 0)
            {
                done--;

                if(done * 5 <= City._Storage._Metal)
                {
                    break;
                }
            }
        }

        City._Storage._Metal -= done * 5;
        City._Storage._Robots += done;
    }
}
