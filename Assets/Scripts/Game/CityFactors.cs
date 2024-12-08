using UnityEngine;
using UnityEngine.SceneManagement;
using static CityFactors;

public class CityFactors : MonoBehaviour
{
    [SerializeField] private CityTime CityTime;
    [SerializeField] private Factor[] SatisfactionFactors;

    private CityMessenger.CityMessage LowSatisfaction = new CityMessenger.CityMessage("<color=red>НАРОД НЕДОВОЛЕН!</color>", "", true);
    private CityMessenger.CityMessage LowHealth = new CityMessenger.CityMessage("<color=red>НАРОД БОЛЕН!</color>", "", true);
    [SerializeField] private int GameOverDaysLeft = 3;
    [SerializeField] private bool Encounting = false;

    private float CitySatisfaction = 0;
    private float SatisfactionFactor = 0;

    public string _SaveInfo
    {
        get
        {
            string info = "S$fn(";

            foreach(Factor factor in SatisfactionFactors)
            {
                info += $"{factor._SaveInfo}|";
            }
            if (info.EndsWith("|"))
            {
                info = info.Remove(info.Length - 1);
            }
            info += ")";

            return info + $"Days({GameOverDaysLeft})Enc({Encounting.GetHashCode()})";
        }
        set
        {
            if(value == null || value == "")
            {
                return;
            }

            string satisfaction = StaticTools.GetParameter(value, "S$fn");

            string[] token = satisfaction.Split('|');

            if (token[0] != "")
            {
                SatisfactionFactors = new Factor[token.Length];
                for (int i = 0; i < token.Length; i++)
                {
                    Factor factor = new Factor();
                    factor._SaveInfo = token[i];
                    SatisfactionFactors[i] = factor;
                }
            }

            Encounting = StaticTools.GetParameter(value, "Enc") == "1";
            if (Encounting)
            {
                GameOverDaysLeft = StaticTools.StringToInt(StaticTools.GetParameter(value, "Days"));
                LowSatisfaction.Info = $"<color=red>Уровень удовлетворённости медведей в городе существенно низок, поэтому медведи готовятся согнать вас с поста мэра.\nВам необходимо в течение {GameOverDaysLeft} дней повысить удовлетворенность медведей до 50% и более.</color>";
                City._CityMessenger.SetMessage(LowSatisfaction, false);
            }

            UpdateSatisfaction();
        }
    }
    public  Factor[] _SatisfactionFactors => SatisfactionFactors;
    public float _CitySatisfaction => CitySatisfaction;
    public int _SatisfactionFactor => Mathf.RoundToInt(SatisfactionFactor);

    private void Start()
    {
        City._DataBase.OnBearChanges += UpdateSatisfaction;

        UpdateSatisfaction();
    }
    
    public void UpdateFactors()
    {
        foreach (Factor factor in SatisfactionFactors)
        {
            factor.Duraction--;
        }

        SatisfactionFactor = 0;
        Factor[] satisfactionFactors = new Factor[0];
        for (int i = 0; i < SatisfactionFactors.Length; i++)
        {
            if (SatisfactionFactors[i].Duraction > 0)
            {
                satisfactionFactors = StaticTools.ExpandMassive(satisfactionFactors, SatisfactionFactors[i]);
                SatisfactionFactor += SatisfactionFactors[i].Value;
            }
        }

        SatisfactionFactors = satisfactionFactors;

        float health = 0;
        foreach(Bear bear in City._DataBase._Bears)
        {
            health += bear._Health;
        }
        health /= City._DataBase._Bears.Length;

        if(health < 3)
        {
            City._CityMessenger.SetMessage(LowHealth, false);
            LowHealth.Info = $"<color=red>Средний показатель здоровья медведей равен {health}, это очень плохо. Если вы ничего не предпримете, то все медведи обречены на криогенное состояние. Тогда ваше капитанство прекратится.</color>";
   
            if(Mathf.Abs(health - 1) < 0.25f)
            {
                SceneManager.LoadScene(4);
                return;
            }
        }
        else
        {
            City._CityMessenger.SetMessage(LowHealth, true);
        }

        UpdateSatisfaction();

        if (Encounting)
        {
            if(CitySatisfaction >= 0.5f)
            {
                Encounting = false;

                City._CityMessenger.SetMessage(LowSatisfaction, true);

                City._CityMessenger.SetMessage(new CityMessenger.CityMessage("Кризис миновал", "Вы смогли стабилизировать ситуацию, впредь будьте аккуратней."), false);
            }
            else
            {
                GameOverDaysLeft--;

                if(GameOverDaysLeft < 1)
                {
                    SceneManager.LoadScene(4);
                    return;
                }

                LowSatisfaction.Info = $"<color=red>Уровень удовлетворённости медведей в городе существенно низок, поэтому медведи готовятся согнать вас с поста капитана.\nВам необходимо в течение {GameOverDaysLeft} дней повысить удовлетворенность медведей до 50% и более.</color>";
            }
        }
        else if(CitySatisfaction < 0.35f && (CitySatisfaction == 0 || Random.Range(0, 100) < 60))
        {
            GameOverDaysLeft = 3;
            Encounting = true;

            LowSatisfaction.Info = $"<color=red>Уровень удовлетворённости медведей в городе существенно низок, поэтому медведи готовятся согнать вас с поста капитана.\nВам необходимо в течение {GameOverDaysLeft} дней повысить удовлетворенность медведей до 50% и более.</color>";
            City._CityMessenger.SetMessage(LowSatisfaction, false);
        }
    }

    public void UpdateSatisfaction()
    {
        CitySatisfaction = 0;
        foreach(Bear bear in City._DataBase._Bears)
        {
            CitySatisfaction += bear._Satisfaction;
        }

        CitySatisfaction /= City._DataBase._Bears.Length * 10;

        if(CitySatisfaction < 0.35f)
        {

        }
    }

    public float GetPotencialSatisfaction()
    {
        float value = 0;
        float saturation = City._Foodstream._Potencial[2];
        foreach (Bear bear in City._DataBase._Bears)
        {
            value += bear.GetSatisfaction(bear._Health, saturation);
        }

        value /= City._DataBase._Bears.Length * 10;

        return value;
    }

    [System.Serializable]
    public class Factor
    {
        public string Name;
        public float Value;
        public int Duraction;
        public bool Coefficient = false;

        public string _SaveInfo
        {
            get
            {
                return $"{Name};{Value};{Duraction}";
            }
            set
            {
                string[] variables = value.Split(';');

                Name = variables[0];
                Value = float.Parse(variables[1]);
                Duraction = int.Parse(variables[2]);
            }
        }

        public Factor() { }

        public Factor(string name, float value, int duraction, bool coefficient = false)
        {
            Name = name;
            Value = value;
            Duraction = duraction;
            Coefficient = coefficient;  
        }

        public override string ToString()
        {
            if (Coefficient)
            {
                return $"{Name}: {Value}x{(Duraction != -1 ? $" (длительность {Duraction} дней)" : "")}";
            }

            return $"{Name}: {(Value > 0 ? "+" : "")}{Value}{( Duraction != -1 ? $" (длительность {Duraction} дней)" : "")}";
        }
    }
}
