using UnityEngine;

public class Aeroport : Facility
{
    [SerializeField] private int DaysLeft;
    [SerializeField] private int MinimalCount;
    [SerializeField] private int MaximalCount;
    private  CityTime CityTime = null;

    private Bear[] LastBears = new Bear[0];

    public override string _SaveInfo
    {
        get => base._SaveInfo + $"DaysLeft({DaysLeft})";
        set
        {
            base._SaveInfo = value;

            DaysLeft = StaticTools.StringToInt(StaticTools.GetParameter(value, "DaysLeft"));
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (CityTime != null)
        {
            CityTime.DayChanged -= DayPassed;
        }
    }

    private void Start()
    {
        CityTime = FindObjectOfType<CityTime>();
        CityTime.DayChanged += DayPassed;
    }

    public void DayPassed()
    {
        DaysLeft--;

        if(DaysLeft <= 0)
        {
            DaysLeft = Random.Range(5, 9);

            string[] maleNames = Resources.Load<TextAsset>("MaleNames").text.Split("\n");
            string[] femaleNames = Resources.Load<TextAsset>("FemaleNames").text.Split("\n");

            int count = Random.Range(MinimalCount, MaximalCount);
            LastBears = new Bear[count];
            string bears = "";
            for (int  i = 0; i < count; i++)
            {
                int age = Random.Range(8, 50);
                int skill = Random.Range(age * 10, 777);
                if(age < 25)
                {
                    skill /= 2;
                }

                bool woman = Random.Range(0, 10) % 2 == 0;
                Bear.Kasta kasta = Bear.Kasta.Неопределёно;
                if(age > 14)
                {
                    kasta = (Bear.Kasta)Random.Range(0, 7);
                }

                Bear newBear = new Bear(kasta, (woman ? femaleNames[Random.Range(0, femaleNames.Length)] : maleNames[Random.Range(0, maleNames.Length)]), skill, Random.Range(5, 9), Random.Range(1, 6), age, woman);

                LastBears[i] = newBear;

                bears += $"\n{newBear._Name} #{City._DataBase._Bears.Length + i}";
            }

            City._DataBase.RegisterBear(LastBears, false);

            City._CityMessenger.SetMessage(new CityMessenger.CityMessage("Пополнение в городе!", $"С аэропорта #{StaticTools.IndexOf(City._DataBase._Facilities, this)} прибыло {count} медведей:{bears}"), false);
        }
    }

    public void AutoAssign(bool state)
    {
        if (state)
        {
            Home home = null;
            int index = 0;
            for(int i = 0; i < City._DataBase._Facilities.Length; i++)
            {
                home = City._DataBase._Facilities[i] as Home;
                if (home != null)
                {
                    if(home._AssignedBears.Length < home._MaxBearCount)
                    {
                        index = i;
                        break;
                    }
                    else
                    {
                        home = null;
                    }
                }
            }

            for(int i = 0; i < LastBears.Length; i++)
            {
                if(home != null)
                {
                    if(!home.AssignBear(LastBears[i], false))
                    {
                        for (int ii = index; ii < City._DataBase._Facilities.Length; ii++)
                        {
                            home = City._DataBase._Facilities[ii] as Home;
                            if (home != null)
                            {
                                if (home._AssignedBears.Length < home._MaxBearCount)
                                {
                                    index = ii;
                                    break;
                                }
                                else
                                {
                                    home = null;
                                }
                            }
                        }

                        if(home != null)
                        {
                            home.AssignBear(LastBears[i], false);
                        }
                    }
                }
            }
        }

        LastBears = new Bear[0];
    }
}
