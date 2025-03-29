using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CityFactors : MonoBehaviour
{
    [SerializeField] private CityTime CityTime;

    [SerializeField] private GameEnder GameEnder;

    [SerializeField] private GameObject DangerOutline;

    [SerializeField] private GameObject StressMessage;
    [SerializeField] private GameObject StressFail;
    [SerializeField] private GameObject BearsFrozed;

    [SerializeField] private Musician.MusicOrder Music;

    [SerializeField] private TimeEditor TimeEditor;
    [SerializeField] private int EndTime;
    [SerializeField] private bool StressTest;

    public int _EndTime => EndTime;
    public bool _StressTest => StressTest;
    public bool _GameEnded => GameEnder._GameEnd;

    public string _SaveInfo
    {
        get
        {
            return $"Test({StressTest.GetHashCode()})Time({EndTime})";
        }
        set
        {
            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            StressTest = parameters["Test"] == "1";
            EndTime = StaticTools.StringToInt(parameters["Time"]);

            if (StressTest)
            {
                DangerOutline.SetActive(true);
            }
        }
    }

    public void HourPassed()
    {
        float stress = 0;
        float health = 0;
        bool deesposobni = false;
        int saturated = City._Foodstream._Saturation >= 1 ? 1 : (City._Foodstream._Saturation <= 0 ? -1 : 0);
        foreach (Bear bear in City._DataBase._Bears)
        {
            bear._Stress += bear._StressDelta * CityTime._DaySection;
            stress += bear._Stress;
            health += bear._Health;

            if (!deesposobni)
            {
                deesposobni = !bear._Nedeesposoben;
            }

            if (bear._Sally == null)
            {
                if (saturated == -1)
                {
                    bear._Health -= Random.Range(0, 100f) < 10 ? 1 : 0;
                }
                if (saturated == 1 && bear._HeatLevel > 0 && bear._Health < 5)
                {
                    bear._Health += Random.Range(0, 100f) < 10 ? 1 : 0;
                }
            }

            if (bear._HeatLevel < -2)
            {
                bear._Health -= Random.Range(0, 100f) < 15 ? 1 : 0;
            }
            else if(bear._HeatLevel < 0)
            {
                bear._Health -= Random.Range(0, 100f) < 5 ? 1 : 0;
            }

            if(bear is SuperBear)
            {
                bear._Health += Random.Range(0, 100f) < (bear as SuperBear)._Regeneration ? 1 : 0;
            }

            for(int i = 0; i < bear._Effects.Length; i++)
            {
                bear._Effects[i] = Mathf.Max(0, bear._Effects[i] - 1);

                if(i == 3 && bear._Effects[i] == 0 && City._Storage._Antisleep > 0)
                {
                    bear._Effects[i] = UnityEngine.Random.Range(120, 150);
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Выдан {bear._Name}", (int)(City._Time._WorldTime / 60), -1, CityStorage.ResourceType.Antisleep));
                    City._Storage._Antisleep--;
                }
            }
        }

        if(City._DataBase._Bears.Length == 0)
        {
            return;
        }

        if(!deesposobni)
        {
            BearsFrozed.SetActive(true);
            DangerOutline.SetActive(true);
            FindObjectOfType<Musician>().SetMusic(Music, false);
            TimeEditor._Paused = true;
            GameEnder._GameEnd = true;

            PlayerPrefs.SetInt("PlayerWinrate", PlayerPrefs.GetInt("PlayerWinrate") - 1);
            PlayerPrefs.Save();
            return;
        }

        stress /= City._DataBase._Bears.Length;
        if (StressTest)
        {
            if(stress <= 50f)
            {
                FindObjectOfType<Musician>().SetMusic(Music, true);
                StressMessage.SetActive(false);
                StressTest = false;
                City._CityMessenger.AddMessage(new CityMessenger.CityMessage("Ситуация стабилизирована", "Стресс медведей спал до определённой нормы, впредь будьте осторожнее."));
            }
            EndTime--;
            if(EndTime <= 0)
            {
                StressFail.SetActive(true);
                GameEnder._GameEnd = true;
                TimeEditor._Paused = true;

                PlayerPrefs.SetInt("PlayerWinrate", PlayerPrefs.GetInt("PlayerWinrate") - 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            if (stress > 90 && Random.Range(0, 100) < 80)
            {
                StressMessage.SetActive(true);
                DangerOutline.SetActive(true);
                FindObjectOfType<Musician>().SetMusic(Music, false);
                TimeEditor._Paused = true;
            }
        }
    }

    public void StartStressTest(bool state)
    {
        StressMessage.SetActive(false);
        if (state)
        {
            StressTest = true;
            EndTime = 50;
            City._CityMessenger.AddMessage(new CityMessenger.CityMessage("Испытание", "Вы пообещали понизить сресс медведей, для этого вы должны проявить себя в самом лучшем виде.\nУ вас есть 50 часов, чтобы средний стресс медведей был ниже 50%."));
        }
        else
        {
            End();
        }
    }

    public void Defeat()
    {
        BearsFrozed.SetActive(true);
        DangerOutline.SetActive(true);
        FindObjectOfType<Musician>().SetMusic(Music, false);
        TimeEditor._Paused = true;
        GameEnder._GameEnd = true;

        PlayerPrefs.SetInt("PlayerWinrate", PlayerPrefs.GetInt("PlayerWinrate") - 1);
        PlayerPrefs.Save();
    }

    public void End()
    {
        PlayerPrefs.SetInt("Exposition", 0);
        PlayerPrefs.Save();

        File.Delete(Path.Combine(Application.persistentDataPath, "LocalSave.json"));
        GameEnder.End(4);
    }

    [System.Serializable]
    public class Factor
    {
        public string Name;
        public float Value;
        public bool Coefficient = false;

        public string _SaveInfo
        {
            get
            {
                return $"{Name};{Value}";
            }
            set
            {
                string[] variables = value.Split(';');

                Name = variables[0];
                Value = float.Parse(variables[1]);
            }
        }

        public Factor() { }

        public Factor(string name, float value, bool coefficient = false)
        {
            Name = name;
            Value = value;
            Coefficient = coefficient;  
        }

        public override string ToString()
        {
            if (Coefficient)
            {
                return $"{Name}: {Value}x";
            }

            return $"{Name}: {(Value > 0 ? "+" : "")}{Value}";
        }
    }
}
