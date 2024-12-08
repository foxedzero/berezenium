using UnityEngine;

public class MedFacility : Facility
{
    [SerializeField] private int Healing;
    [SerializeField] private int HealCount;

    public override bool _CanBeDisabled => false;

    public int _Healing => Healing + (City._Research.GetResearchLevel(CityResearch.ResearchType.Medicine) >= 2 ? 1 : 0);
    public int _HealCount => HealCount + (City._Research.GetResearchLevel(CityResearch.ResearchType.Medicine) >= 2 ? 3 : 0);

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
        Bear[] bears = City._DataBase._Bears;
        if(bears.Length < 1)
        {
            return;
        }

        int[] minimal = new int[Mathf.RoundToInt(Mathf.Clamp(_HealCount * _Effectivity, 1, bears.Length))];

        if(bears.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < bears.Length; i++)
        {
            if (bears[minimal[0]]._Health > bears[i]._Health)
            {
                for(int ii = 0; ii < minimal.Length - 1; ii++)
                {
                    minimal[ii + 1] = minimal[ii];
                }

                minimal[0] = i;
            }
        }

        for(int i = 0; i < minimal.Length; i++)
        {
            bears[minimal[i]]._Health += _Healing * (bears[minimal[i]]._Health < 4 ? 2 : 1);
        }
    }
}
