using UnityEngine;

public class Weather : MonoBehaviour
{
    [SerializeField] private Camera[] Cameras;
    [SerializeField] private Material[] Clouds;
    [SerializeField] private Light GlobalLight;
    [SerializeField] private bool Osadki;
    [SerializeField] private int ColdLevel;

    [SerializeField] private GameObject OsadkiEffect;

    public int _Cold => ColdLevel;
    public string _SaveInfo
    {
        get
        {
            return $"{ColdLevel};{Osadki.GetHashCode()}";
        }
        set
        {
            string[] tokens = value.Split(';');
            ColdLevel = StaticTools.StringToInt(tokens[0]);
            Osadki = tokens[1] == "1";

            OsadkiEffect.SetActive(Osadki);
        }
    }

    public void NewDay()
    {
        foreach(Bear bear in City._DataBase._Bears)
        {
            int temperatureDelta = bear._ColdEndurance - ColdLevel;
            if(temperatureDelta < -5)
            {
                bear._Health -= 2;
            }
            else if(temperatureDelta < -2)
            {
                bear._Health -= 1;
            }
            else if(temperatureDelta > 0 && bear._Health < 6)
            {
                if(City._Foodstream._Saturation > 0.75f)
                {
                    bear._Health += 1;
                }
            }
        }

        int heatBonus = 0;

        switch (City._Time._Month)
        {
            case 0:
                heatBonus = 2;
                Osadki = Random.Range(0, 100f) <= 70;
                break;
            case 1:
                heatBonus = 3;
                Osadki = Random.Range(0, 100f) <= 50;
                break;
            case 2:
                heatBonus = 4;
                Osadki = Random.Range(0, 100f) <= 40;
                break;
            case 3:
                heatBonus = 5;
                Osadki = Random.Range(0, 100f) <= 40;
                break;
            case 4:
                heatBonus = 5;
                Osadki = Random.Range(0, 100f) <= 45;
                break;
            case 5:
                heatBonus = 6;
                Osadki = Random.Range(0, 100f) <= 35;
                break;
            case 6:
                heatBonus = 4;
                Osadki = Random.Range(0, 100f) <= 40;
                break;
            case 7:
                heatBonus = 3;
                Osadki = Random.Range(0, 100f) <= 45;
                break;
            case 8:
                heatBonus = 2;
                Osadki = Random.Range(0, 100f) <= 50;
                break;
            case 9:
                heatBonus = 2;
                Osadki = Random.Range(0, 100f) <= 65;
                break;
            case 10:
                heatBonus = 1;
                Osadki = Random.Range(0, 100f) <= 70;
                break;
            case 11:
                heatBonus = 0;
                Osadki = Random.Range(0, 100f) <= 90;
                break;
        }

        OsadkiEffect.SetActive(Osadki);

        ColdLevel = Random.Range(7, 9) - heatBonus + (Osadki ? 2 : 0);
    }

    private void Update()
    {
        float time = City._Time._DayProgress;
        if (time < 0.2f || time > 0.92f)
        {
            time = 0;
        }
        else if(time < 0.48f)
        {
            time = Mathf.InverseLerp(0.2f, 0.48f, time);
        }
        else if(time > 0.6f)
        {
            time = 1 - Mathf.InverseLerp(0.6f, 0.92f, time);
        }
        else
        {
            time = 1;
        }

        if (Osadki)
        {
            time = Mathf.Clamp(time, 0, 0.25f);
        }

        Color color = Color.HSVToRGB(0, 0, Mathf.Lerp(0.1f, 0.5f, time));
        foreach (Camera camera in Cameras)
        {
            camera.backgroundColor = color;
        }

        color = Color.HSVToRGB(0.62f, 0.13f, Mathf.Lerp(0.3f, 0.87f, time));
        color.a = 0.75f;
        Clouds[0].SetColor("_CloudColor", color);
       
        color = Color.HSVToRGB(0.62f, 0.13f, Mathf.Lerp(0.13f, 0.56f, time));
        color.a = 0.40f;
        Clouds[1].SetColor("_CloudColor", color);

        GlobalLight.transform.localEulerAngles = new Vector3(Mathf.Lerp(30, 50, time), Mathf.Lerp(-100, 100, City._Time._DayProgress), 0);
        GlobalLight.intensity = 1.2f * time;
    }
}
