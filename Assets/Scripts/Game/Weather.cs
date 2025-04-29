using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    [SerializeField] private Camera[] Cameras;
    [SerializeField] private Material[] Clouds;
    [SerializeField] private Light GlobalLight;
    [SerializeField] private Settings Settings;

    [SerializeField] private GameObject CloudsSky;

    [SerializeField] private float Osadki;
    [SerializeField] private float SnowSpeed;
    [SerializeField] private float Mist;
    [SerializeField] private int ColdLevel;

    [SerializeField] private ParticleSystem SnowParticle;

    [SerializeField] private GameObject OsadkiEffect;

    [SerializeField] private GameObject[] Freezers = new GameObject[0];

    public float _Mist => Mist;
    public float _Osadki => Osadki;
    public int _Cold => ColdLevel + GlobalVariables._ColdAddset;
    public string _SaveInfo
    {
        get
        {
            return $"Cold({ColdLevel})";
        }
        set
        {
            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            ColdLevel = StaticTools.StringToInt(parameters["Cold"]);


        }
    }

    private void Start()
    {
        Settings.OnChanges += UpdateSnowParticle;

        if (City._Time._WorldTime / 1500 > 75)
        {
            Osadki = Random.Range(ColdLevel / 14f * 100 - 25, ColdLevel / 14f * 100);
            SnowSpeed = Random.Range(ColdLevel * 2 - 2, ColdLevel * 2 + 2);
            Mist = Random.Range(Mathf.Max(0, ColdLevel / 14f - 0.2f), ColdLevel / 14f);
        }
        else if (City._Time._WorldTime / 1500 > 70)
        {
            Osadki = 100f;
            SnowSpeed = Random.Range(40f, 45f);
            Mist = 1;
        }
        else if (City._Time._WorldTime / 1500 > 65)
        {
            Osadki = Random.Range(90, 100f);
            SnowSpeed = Random.Range(20f, 35f);
            Mist = Random.Range(0.85f, 1f);
        }
        else if (City._Time._WorldTime / 1500 > 60)
        {
            Osadki = Random.Range(80f, 100f);
            SnowSpeed = Random.Range(13f, 20f);
            Mist = Random.Range(0.85f, 1f);
        }
        else if (City._Time._WorldTime / 1500 > 55)
        {
            Osadki = Random.Range(50f, 90f);
            SnowSpeed = Random.Range(10f, 13f);
            Mist = Random.Range(0.6f, 0.9f);
        }
        else if (City._Time._WorldTime / 1500 > 50)
        {
            Osadki = Random.Range(35, 80f);
            SnowSpeed = Random.Range(5f, 8f);
            Mist = Random.Range(0.4f, 0.7f);
        }
        else if (City._Time._WorldTime / 1500 > 45)
        {
            Osadki = Random.Range(35f, 60f);
            SnowSpeed = Random.Range(4f, 5f);
            Mist = Random.Range(0.25f, 0.5f);
        }
        else if (City._Time._WorldTime / 1500 > 40)
        {
            Osadki = Random.Range(10, 50f);
            SnowSpeed = Random.Range(1f, 2f);
            Mist = Random.Range(0, 0.1f);
        }
        else if (City._Time._WorldTime / 1500 > 35)
        {
            Osadki = Random.Range(70f, 100f);
            SnowSpeed = Random.Range(1f, 2f);
            Mist = Random.Range(0, 0.75f);
        }
        else if (City._Time._WorldTime / 1500 > 30)
        {
            Osadki = Random.Range(40, 45);
            SnowSpeed = Random.Range(1f, 2f);
            Mist = Random.Range(0.6f, 0.8f);
        }
        else if (City._Time._WorldTime / 1500 > 25)
        {
            Osadki = Random.Range(80, 100f);
            SnowSpeed = Random.Range(10f, 20f);
            Mist = Random.Range(0.8f, 1f);
        }
        else if (City._Time._WorldTime / 1500 > 20)
        {
            Osadki = Random.Range(60f, 90f);
            SnowSpeed = Random.Range(8f, 12f);
            Mist = Random.Range(0.4f, 0.8f);
        }
        else if (City._Time._WorldTime / 1500 > 15)
        {
            Osadki = Random.Range(40f, 80f);
            SnowSpeed = Random.Range(3, 5f);
            Mist = Random.Range(0.6f, 0.7f);
        }
        else if (City._Time._WorldTime / 1500 > 10)
        {
            Osadki = Random.Range(50, 90f);
            SnowSpeed = Random.Range(3f, 4f);
            Mist = Random.Range(0.25f, 0.7f);
        }
        else if (City._Time._WorldTime / 1500 > 5)
        {
            Osadki = Random.Range(25f, 80f);
            SnowSpeed = Random.Range(4f, 7f);
            Mist = Random.Range(0.15f, 0.5f);
        }
        else
        {
            Osadki = Random.Range(0, 35f);
            SnowSpeed = Random.Range(3f, 5f);
            Mist = Random.Range(0, 0.2f);
        }

        Osadki /= 100f;
        UpdateSnowParticle();
    }

    public void FreezeSnow(GameObject gameObject, bool remove)
    {
        if (remove)
        {
            Freezers = StaticTools.RemoveFromMassive(Freezers, gameObject);
        }
        else
        {
            Freezers = StaticTools.ExcludingExpandMassive(Freezers, gameObject);
        }

        if (Freezers.Length > 0)
        {
            SnowParticle.Pause();
        }
        else
        {
            SnowParticle.Play();
        }
    }

    public void NewDay()
    {
        if (City._Time._WorldTime / 1500 > 75)
        {
            Mathf.RoundToInt(Mathf.Lerp(3, 11, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));

            Osadki = Mathf.RoundToInt(Mathf.Lerp(ColdLevel / 14f * 100 - 25, ColdLevel / 14f * 100, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(ColdLevel * 2 - 2, ColdLevel * 2 + 2, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(Mathf.Max(0, ColdLevel / 14f - 0.2f), ColdLevel / 14f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if(City._Time._WorldTime / 1500 > 70)
        {
            ColdLevel = 13;
            Osadki = 100f; 
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(40, 45, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
         
            Mist = 1;
        }
        else if (City._Time._WorldTime / 1500 > 65)
        {
            Mathf.RoundToInt(Mathf.Lerp(10, 12, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
          
            Osadki = Mathf.RoundToInt(Mathf.Lerp(90, 100f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(20, 35, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.85f, 1, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 60)
        {
            Mathf.RoundToInt(Mathf.Lerp(9, 11, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
    
            Osadki = Mathf.RoundToInt(Mathf.Lerp(80, 100f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(13, 20, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.85f, 1, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 55)
        {
            Mathf.RoundToInt(Mathf.Lerp(9, 10, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
      
            Osadki = Mathf.RoundToInt(Mathf.Lerp(50, 90f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(10, 13, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.6f, 0.9f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 50)
        {
            Mathf.RoundToInt(Mathf.Lerp(5, 8, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
      
            Osadki = Mathf.RoundToInt(Mathf.Lerp(35, 80f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(5, 8, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.4f, 0.7f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 45)
        {
            Mathf.RoundToInt(Mathf.Lerp(5, 6, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
      
            Osadki = Mathf.RoundToInt(Mathf.Lerp(35, 60f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(4, 5, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.25f, 0.5f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 40)
        {
            Mathf.RoundToInt(Mathf.Lerp(4, 6, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
       
            Osadki = Mathf.RoundToInt(Mathf.Lerp(10, 50f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(1, 2, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0, 0.1f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 35)
        {
            Mathf.RoundToInt(Mathf.Lerp(3, 4, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
      
            Osadki = Mathf.RoundToInt(Mathf.Lerp(70, 100f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(1, 2, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0, 0.75f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 30)
        {
            Mathf.RoundToInt(Mathf.Lerp(8, 9, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
     
            Osadki = Mathf.RoundToInt(Mathf.Lerp(40, 45, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(1, 2, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.6f, 0.8f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 25)
        {
            Mathf.RoundToInt(Mathf.Lerp(7, 8, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
      
            Osadki = Mathf.RoundToInt(Mathf.Lerp(80, 100f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(10f, 20f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.8f, 1, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 20)
        {
            Mathf.RoundToInt(Mathf.Lerp(5, 6, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));

            Osadki = Mathf.RoundToInt(Mathf.Lerp(60, 90f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(8, 12f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.4f, 0.8f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 15)
        {
            Mathf.RoundToInt(Mathf.Lerp(4, 5, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
           
            Osadki = Mathf.RoundToInt(Mathf.Lerp(40, 80f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(3, 5, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.6f, 0.7f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 10)
        {
            Mathf.RoundToInt(Mathf.Lerp(3, 5, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Osadki = Mathf.RoundToInt(Mathf.Lerp(50, 90f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(3, 4, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.25f, 0.7f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else if (City._Time._WorldTime / 1500 > 5)
        {
            ColdLevel = Mathf.RoundToInt(Mathf.Lerp(2, 4, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Osadki = Mathf.RoundToInt(Mathf.Lerp(25, 80f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(4, 7, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0.15f, 0.5f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }
        else 
        {
            ColdLevel = Mathf.RoundToInt(Mathf.Lerp(2, 3, ProceduralTools.PseudoRandom((SaveManager._Seed + $"cold{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Osadki = Mathf.RoundToInt(Mathf.Lerp(0, 35f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"osadki{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            SnowSpeed = Mathf.RoundToInt(Mathf.Lerp(3, 5, ProceduralTools.PseudoRandom((SaveManager._Seed + $"snowSpd{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
            Mist = Mathf.RoundToInt(Mathf.Lerp(0, 0.2f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"mist{Mathf.RoundToInt(City._Time._WorldTime / 1500)}").GetHashCode())));
        }

        Osadki /= 100f;
        UpdateSnowParticle();
    }

    public void UpdateSnowParticle()
    {
        SnowParticle.startLifetime = 100 / SnowSpeed;
        SnowParticle.startSpeed = SnowSpeed;
        SnowParticle.maxParticles = Settings._Data.MaxSnowCount;
        SnowParticle.emissionRate = (Settings._Data.MaxSnowCount / SnowParticle.startLifetime) * Osadki;

        RenderSettings.fogEndDistance = Mathf.Lerp(10, 300, 1 - Mist);
        CloudsSky.SetActive(Mist < 0.6f);
        foreach (Camera camera in Cameras)
        {
            camera.backgroundColor = RenderSettings.fogColor;
        }
    }

    private void Update()
    {
        float dayProgress = (City._Time._WorldTime % 1500) / 1500f;
        float time = dayProgress;
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

        time = Mathf.Clamp(time, 0, 1 - Mist);

        if(Mist < 0.6f)
        {
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
        }

        GlobalLight.transform.localEulerAngles = new Vector3(Mathf.Lerp(30, 50, time), Mathf.Lerp(-100, 100, dayProgress), 0);
        GlobalLight.intensity = 1.2f * time;
    }
}
