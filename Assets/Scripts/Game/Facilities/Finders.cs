using UnityEngine;

public class Finders : Facility
{
    [SerializeField] private float FindProgress;

    public override bool _CanBeDisabled => false;
    public override bool _CanBeHeated => false;

    public float _Progress => FindProgress;

    public override string _SaveInfo
    {
        get => base._SaveInfo + $"Progress({FindProgress})Hardness({RequiredSkill})";
        set
        {
            base._SaveInfo = value;

            FindProgress = StaticTools.StringToFloat(StaticTools.GetParameter(value, "Progress"));
            RequiredSkill = StaticTools.StringToFloat(StaticTools.GetParameter(value, "Hardness"));
        }
    }

    public override int _ColdEndurance => base._ColdEndurance + (City._Research.GetResearchLevel(CityResearch.ResearchType.Travels) >= 1 ? 3 : 0);
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        if(City._Research.GetResearchLevel(CityResearch.ResearchType.Travels) >= 2)
        {
            return StaticTools.ExpandMassive(base.GetEffectivityFactors(), new CityFactors.Factor("Снегоходы", 1.5f, -1, true));
        }
        else
        {
            return base.GetEffectivityFactors();
        }
    }
    public override float GetPotencialEffectivity()
    {
        return base.GetPotencialEffectivity() * (City._Research.GetResearchLevel(CityResearch.ResearchType.Travels) >= 2 ? 1.5f : 1);
    }
    public override float _Effectivity => base._Effectivity * (City._Research.GetResearchLevel(CityResearch.ResearchType.Travels) >= 2 ? 1.5f : 1);

    private void Start()
    {
        City._Time.DayChanged += DayPassed;

        if(FindProgress == 0)
        {
            FindProgress = 1 + (City._DataBase._Bears.Length - 12) * 0.1f;
            RequiredSkill = 200 + (City._DataBase._Bears.Length - 12) * 20;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        City._Time.DayChanged -= DayPassed;
    }

    public void DayPassed()
    {
        FindProgress -= _Effectivity;

        if (FindProgress <= 0)
        {
            FindProgress = 1 + (City._DataBase._Bears.Length - 12) * 0.1f;
            RequiredSkill = 200 + (City._DataBase._Bears.Length - 12) * 20;

            int health = 0;

            if(City._Time._Time > 30)
            {
                health = 1;
            }
            else if (City._Time._Time > 15)
            {
                health = Random.Range(2, 5);
            }
            else if (City._Time._Time > 5)
            {
                health = Random.Range(3, 6);
            }
            else
            {
                health = Random.Range(4, 8);
            }

            string[] maleNames = Resources.Load<TextAsset>("MaleNames").text.Split("\n");
            string[] femaleNames = Resources.Load<TextAsset>("FemaleNames").text.Split("\n");

            int age = Random.Range(16, 30);
            int skill = Random.Range(250 + (int)City._Time._Time * 5, 400 + (int)City._Time._Time * 10);

            bool woman = Random.Range(0, 10) % 2 == 0;

            Bear.Kasta kasta = (Bear.Kasta)Random.Range(1, 6);

            Bear newBear = new Bear(kasta, (woman ? femaleNames[Random.Range(0, femaleNames.Length)] : maleNames[Random.Range(0, maleNames.Length)]), skill, health, Random.Range(4, 9), age, woman);

            City._DataBase.RegisterBear(newBear, false);

            CityMessenger.CityMessage message = new CityMessenger.CityMessage("Спасён медведь", $"Наши разведчики смогли найти члена экипажа - {newBear._Name}.");
            message.Fest = true;
            City._CityMessenger.SetMessage(message, false);
        }
    }
    
}
