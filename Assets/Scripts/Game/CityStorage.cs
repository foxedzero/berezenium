using UnityEngine;

public class CityStorage : MonoBehaviour
{
    public enum ResourceType { EnergyHoney , Berezenium , Metal , Wood , Robots, Honey, None = -1 }

    [SerializeField] private float EnergyHoney;
    [SerializeField] private float Berezenium;
    [SerializeField] private float Metal;
    [SerializeField] private float Wood;
    [SerializeField] private int Robots;

    public event SimpleVoid OnChanges = null;

    public float _EnergyHoney
    {
        get
        {
            return EnergyHoney;
        }
        set
        {
            EnergyHoney = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public float _Berezenium
    {
        get
        {
            return Berezenium;
        }
        set
        {
            Berezenium = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public float _Metal
    {
        get
        {
            return Metal;
        }
        set
        {
            Metal = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public float _Wood
    {
        get
        {
            return Wood;
        }
        set
        {
            Wood = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }
    public int _Robots
    {
        get
        {
            return Robots;
        }
        set
        {
            Robots = Mathf.Max(0, value);

            if (OnChanges != null)
            {
                OnChanges.Invoke();
            }
        }
    }

    public string _SaveInfo
    {
        get
        {
            return $"{EnergyHoney};{Berezenium};{Metal};{Wood};{Robots}";
        }
        set
        {
            string[] tokens = value.Split(";");

            EnergyHoney = StaticTools.StringToFloat (tokens[0]);
            Berezenium = StaticTools.StringToFloat(tokens[1]);
            Metal = StaticTools.StringToFloat(tokens[2]);
            Wood = StaticTools.StringToFloat(tokens[3]);
            Robots = StaticTools.StringToInt(tokens[4]);
        }
    }

    public static string ResourceName(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.EnergyHoney:
                return "Энергомёд";
            case ResourceType.Berezenium:
                return "Березениум";
            case ResourceType.Metal:
                return "Металл";
            case ResourceType.Wood:
                return "Древесина";
            case ResourceType.Robots:
                return "Роботы";
        }

        return "";
    }
}
